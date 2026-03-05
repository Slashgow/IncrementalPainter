using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityTimer;

public class Vacuumer : MonoBehaviour, IVacuumer
{
    [Header("Vacuum Configuration")]
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> vacuumForcePerLevel;
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> vacuumRangePerLevel;

    public SkillDataPerLevelOfType<FunctionAffine> VacuumForcePerLevel => vacuumForcePerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> VacuumRangePerLevel => vacuumRangePerLevel;

    [Header("Detection")]
    [SerializeField] private LayerMask vacuumableLayers = ~0;
    [SerializeField] private float detectionInterval = 0.1f;
    [SerializeField] private int detectionBufferSize = 64;

    [Header("Collection")]
    [SerializeField] private float collectDistance = 0.5f;

    [Header("Settings")]
    [SerializeField] private bool isActive = true;

    [Header("Performance")]
    [SerializeField, Range(1, 8)] private int updateSlices = 4;

    public Transform Transform => transform;
    public float VacuumForce => vacuumForcePerLevel.GetCurrentLevelData() + vacuumForceBoost;
    public float VacuumRange => vacuumRangePerLevel.GetCurrentLevelData() + vacuumRangeBoost;
    public float CollectDistance => collectDistance;
    public bool IsActive => isActive;

    private float vacuumForceBoost = 0f;
    private float vacuumRangeBoost = 0f;
    private Timer boostDuration;

    // ── Cached collections — no per-frame allocations ────────────────────────
    private readonly HashSet<IVacuumable> vacuumedObjects = new();
    private readonly HashSet<IVacuumable> _objectsInRangeBuffer = new();
    private readonly List<IVacuumable> _vacuumablesCopy = new();
    private readonly Dictionary<Collider2D, IVacuumable> _componentCache = new();
    private Collider2D[] _overlapBuffer;

    // ── Spread update state ──────────────────────────────────────────────────
    private int _updateSlice = 0;

    private Timer detectionTimer;

    public UnityEvent OnVacuumerActivated;
    public UnityEvent OnVacuumerDeactivated;
    public UnityEvent<IVacuumable> OnVacuumableCollected;

    public static event Action<IVacuumer, IVacuumable> OnAnyVacuumerCollect;

    public void AddTemporaryBoost(float forceBoost, float rangeBoost, float duration)
    {
        boostDuration?.Cancel();
        boostDuration = Timer.Register(
            duration,
            onComplete: () => { vacuumForceBoost = 0f; vacuumRangeBoost = 0f; },
            useRealTime: false
        );
        vacuumForceBoost = forceBoost;
        vacuumRangeBoost = rangeBoost;
    }

    private void Start()
    {
        _overlapBuffer = new Collider2D[detectionBufferSize];
        StartDetectionTimer();
    }

    private void StartDetectionTimer()
    {
        detectionTimer = Timer.Register(detectionInterval, onComplete: DetectVacuumableObjects, isLooped: true, useRealTime: false);
    }

    private void Update()
    {
        if (!isActive) return;
        ApplyVacuumMovement();
    }

    private void DetectVacuumableObjects()
    {
        if (!isActive) return;

        // NonAlloc — no heap allocation
        int count = Physics2D.OverlapCircleNonAlloc(transform.position, VacuumRange, _overlapBuffer, vacuumableLayers);

        _objectsInRangeBuffer.Clear();

        for (int i = 0; i < count; i++)
        {
            IVacuumable vacuumable = GetCachedVacuumable(_overlapBuffer[i]);
            if (vacuumable != null && VacuumUtility.IsValidVacuumable(vacuumable))
            {
                _objectsInRangeBuffer.Add(vacuumable);
                vacuumedObjects.Add(vacuumable); // HashSet.Add is a no-op if already present
            }
        }

        // Remove objects no longer valid or in range
        vacuumedObjects.RemoveWhere(v => !VacuumUtility.IsValidVacuumable(v) || !_objectsInRangeBuffer.Contains(v));
    }

    private void ApplyVacuumMovement()
    {
        // Cache properties — avoids calling GetCurrentLevelData() for every object
        float force = VacuumForce;
        float collectSqr = collectDistance * collectDistance; // sqrMagnitude avoids sqrt
        Vector3 myPos = transform.position;
        float dt = Time.deltaTime;

        _vacuumablesCopy.Clear();
        _vacuumablesCopy.AddRange(vacuumedObjects);

        int total = _vacuumablesCopy.Count;
        if (total == 0) return;

        // All objects get movement every frame — spread only applies to validity checks
        for (int i = 0; i < total; i++)
        {
            IVacuumable vacuumable = _vacuumablesCopy[i];

            // Spread: only validate & clean up stale refs on this frame's slice
            if (i % updateSlices == _updateSlice && !VacuumUtility.IsValidVacuumable(vacuumable))
            {
                vacuumedObjects.Remove(vacuumable);
                continue;
            }

            if (!vacuumable.CanBeVacuumed) continue;

            Vector3 currentPos = vacuumable.Transform.position;
            float sqrDist = (currentPos - myPos).sqrMagnitude;

            if (sqrDist <= collectSqr)
            {
                CollectVacuumable(vacuumable);
                continue;
            }

            Vector3 movement = VacuumUtility.CalculateVacuumMovement(currentPos, myPos, force, vacuumable.VacuumResistance, dt);
            vacuumable.ApplyVacuumMovement(movement);
        }

        _updateSlice = (_updateSlice + 1) % updateSlices;
    }

    private IVacuumable GetCachedVacuumable(Collider2D col)
    {
        if (!_componentCache.TryGetValue(col, out IVacuumable v))
        {
            v = col.GetComponentInParent<IVacuumable>();
            _componentCache[col] = v; // cache null too, to avoid re-searching
        }
        return v;
    }

    public void CollectVacuumable(IVacuumable vacuumable)
    {
        if (!VacuumUtility.IsValidVacuumable(vacuumable)) return;

        vacuumedObjects.Remove(vacuumable);
        RemoveFromCache(vacuumable);

        vacuumable.OnVacuumedCollect(this);
        OnVacuumableCollected?.Invoke(vacuumable);
        OnAnyVacuumerCollect?.Invoke(this, vacuumable);
    }

    private void RemoveFromCache(IVacuumable vacuumable)
    {
        // Clean up any collider entries that pointed to this vacuumable
        var toRemove = new List<Collider2D>();
        foreach (var kvp in _componentCache)
            if (kvp.Value == vacuumable) toRemove.Add(kvp.Key);
        foreach (var key in toRemove)
            _componentCache.Remove(key);
    }

    public void SetActive(bool active)
    {
        bool wasActive = isActive;
        isActive = active;

        if (isActive && !wasActive)
        {
            OnVacuumerActivated?.Invoke();
        }
        else if (!isActive && wasActive)
        {
            OnVacuumerDeactivated?.Invoke();
            vacuumedObjects.Clear();
        }
    }

    private void OnDestroy()
    {
        detectionTimer?.Cancel();
        boostDuration?.Cancel();
        vacuumedObjects.Clear();
        _componentCache.Clear();
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = isActive ? new Color(0f, 1f, 1f, 0.3f) : Color.gray;
        Gizmos.DrawWireSphere(transform.position, VacuumRange);

        Gizmos.color = isActive ? new Color(1f, 1f, 0f, 0.5f) : Color.gray;
        Gizmos.DrawWireSphere(transform.position, CollectDistance);

        if (isActive)
        {
            Gizmos.color = Color.cyan;
            foreach (var vacuumable in vacuumedObjects)
                if (VacuumUtility.IsValidVacuumable(vacuumable))
                    Gizmos.DrawLine(transform.position, vacuumable.Transform.position);
        }
    }
}