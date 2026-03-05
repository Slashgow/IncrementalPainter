using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityTimer;

public class Magneter : MonoBehaviour, IMagneter
{
    [Header("Detection")]
    [SerializeField] private LayerMask magnetableLayers = ~0;
    [SerializeField] private float detectionInterval = 0.2f;
    [SerializeField] private int detectionBufferSize = 512;

    [Header("Performance")]
    [SerializeField, Range(1, 8)] private int updateSlices = 4;

    [Header("Settings")]
    [SerializeField] private bool isActive = true;
    [SerializeField] private bool useCustomMagnetData = false;
    [SerializeField] private MagnetData customMagnetData;

    public Transform Transform => transform;

    public float AttractionForce => useCustomMagnetData && customMagnetData != null
        ? customMagnetData.AttractionForce
        : MagneterManager.Instance.AttractionForcePerLevel.GetCurrentLevelData();

    public float AttractionRadius => useCustomMagnetData && customMagnetData != null
        ? customMagnetData.AttractionRadius
        : MagneterManager.Instance.AttractionRadiusPerLevel.GetCurrentLevelData();

    public float OrbitRadius => useCustomMagnetData && customMagnetData != null
        ? customMagnetData.OrbitRadius
        : MagneterManager.Instance.OrbitRadiusPerLevel.GetCurrentLevelData();

    public float OrbitSpeed => useCustomMagnetData && customMagnetData != null
        ? customMagnetData.OrbitSpeed
        : MagneterManager.Instance.OrbitSpeedPerLevel.GetCurrentLevelData();

    public bool IsActive => isActive;

    // ── Cached collections — no per-frame allocations ────────────────────────
    private readonly HashSet<IMagnetable> magnetedObjects = new();
    private readonly Dictionary<IMagnetable, float> orbitAngles = new();
    private readonly HashSet<IMagnetable> _objectsInRangeBuffer = new();
    private readonly List<IMagnetable> _magnetablesCopy = new();
    private readonly Dictionary<Collider2D, IMagnetable> _componentCache = new();
    private Collider2D[] _overlapBuffer;
    private int _updateSlice = 0;

    private Timer detectionTimer;

    public UnityEvent OnMagnetActivated;
    public UnityEvent OnMagnetDeactivated;
    public static event Action<IMagneter, IMagnetable> OnAnyMagnetAttract;
    public static event Action<IMagneter, IMagnetable> OnAnyMagnetRelease;

    private void Start()
    {
        _overlapBuffer = new Collider2D[detectionBufferSize];
        StartDetectionTimer();
    }

    private void StartDetectionTimer()
    {
        detectionTimer = Timer.Register(detectionInterval, onComplete: DetectMagnetableObjects, isLooped: true, useRealTime: false);
    }

    private void Update()
    {
        if (!isActive) return;
        ApplyMagneticMovement();
    }

    private void DetectMagnetableObjects()
    {
        if (!isActive) return;

        // NonAlloc + auto-resize if buffer was too small
        int count = Physics2D.OverlapCircleNonAlloc(transform.position, AttractionRadius, _overlapBuffer, magnetableLayers);
        while (count == _overlapBuffer.Length)
        {
            _overlapBuffer = new Collider2D[_overlapBuffer.Length * 2];
            count = Physics2D.OverlapCircleNonAlloc(transform.position, AttractionRadius, _overlapBuffer, magnetableLayers);
        }

        _objectsInRangeBuffer.Clear();

        for (int i = 0; i < count; i++)
        {
            IMagnetable magnetable = GetCachedMagnetable(_overlapBuffer[i]);
            if (magnetable != null && MagnetUtility.IsValidMagnetable(magnetable))
            {
                _objectsInRangeBuffer.Add(magnetable);
                if (!magnetedObjects.Contains(magnetable))
                    AddMagnetableObject(magnetable);
            }
        }

        magnetedObjects.RemoveWhere(magnetable =>
        {
            if (!MagnetUtility.IsValidMagnetable(magnetable) || !_objectsInRangeBuffer.Contains(magnetable))
            {
                RemoveMagnetableObject(magnetable);
                return true;
            }
            return false;
        });
    }

    private void ApplyMagneticMovement()
    {
        // Cache properties once — avoids calling GetCurrentLevelData() for every object
        float attractionForce = AttractionForce;
        float orbitRadius = OrbitRadius;
        float orbitRadiusSqr = orbitRadius * orbitRadius;
        float orbitSpeed = OrbitSpeed;
        Vector3 myPos = transform.position;
        float dt = Time.deltaTime;

        _magnetablesCopy.Clear();
        _magnetablesCopy.AddRange(magnetedObjects);

        for (int i = 0; i < _magnetablesCopy.Count; i++)
        {
            IMagnetable magnetable = _magnetablesCopy[i];

            // Spread: validity check seulement sur 1/updateSlices des objets par frame
            if (i % updateSlices == _updateSlice && !MagnetUtility.IsValidMagnetable(magnetable))
            {
                RemoveMagnetableObject(magnetable);
                continue;
            }

            Vector3 currentPos = magnetable.Transform.position;
            float sqrDist = (currentPos - myPos).sqrMagnitude;

            if (sqrDist < 0.0001f) // 0.01f squared
                continue;

            Vector3 movement;

            if (sqrDist > orbitRadiusSqr)
                movement = MagnetUtility.CalculatePullMovement(currentPos, myPos, attractionForce, magnetable.MagnetResistance, dt);
            else
                movement = CalculateOrbitMovement(magnetable, Mathf.Sqrt(sqrDist), orbitRadius, orbitSpeed, attractionForce, myPos, dt);

            magnetable.ApplyMagneticMovement(movement);
        }

        _updateSlice = (_updateSlice + 1) % updateSlices;
    }

    private Vector3 CalculateOrbitMovement(IMagnetable magnetable, float currentDistance, float orbitRadius, float orbitSpeed, float attractionForce, Vector3 myPos, float dt)
    {
        // TryGetValue = single lookup instead of ContainsKey + indexer (two lookups)
        if (!orbitAngles.TryGetValue(magnetable, out float angle))
        {
            Vector3 relativePos = magnetable.Transform.position - myPos;
            angle = MagnetUtility.CalculateAngleFromPosition(relativePos);
        }

        angle += MagnetUtility.CalculateOrbitAngleIncrement(orbitSpeed, dt);
        orbitAngles[magnetable] = angle;

        return MagnetUtility.CalculateOrbitMovement(magnetable.Transform.position, myPos, angle, orbitRadius, orbitSpeed,
            attractionForce, magnetable.MagnetResistance, dt);
    }

    private IMagnetable GetCachedMagnetable(Collider2D col)
    {
        if (!_componentCache.TryGetValue(col, out IMagnetable m))
        {
            m = col.GetComponentInParent<IMagnetable>();
            _componentCache[col] = m; // cache null too, to avoid re-searching
        }
        return m;
    }

    public void AddMagnetableObject(IMagnetable magnetable)
    {
        if (magnetedObjects.Add(magnetable))
        {
            magnetable.OnMagnetEnter(this);
            OnAnyMagnetAttract?.Invoke(this, magnetable);
        }
    }

    public void RemoveMagnetableObject(IMagnetable magnetable)
    {
        if (magnetedObjects.Remove(magnetable))
        {
            orbitAngles.Remove(magnetable);
            RemoveFromCache(magnetable);
            magnetable.OnMagnetExit(this);
            OnAnyMagnetRelease?.Invoke(this, magnetable);
        }
    }

    private void RemoveFromCache(IMagnetable magnetable)
    {
        var toRemove = new List<Collider2D>();
        foreach (var kvp in _componentCache)
            if (kvp.Value == magnetable) toRemove.Add(kvp.Key);
        foreach (var key in toRemove)
            _componentCache.Remove(key);
    }

    public void SetActive(bool active)
    {
        bool wasActive = isActive;
        isActive = active;

        if (isActive && !wasActive)
        {
            OnMagnetActivated?.Invoke();
        }
        else if (!isActive && wasActive)
        {
            OnMagnetDeactivated?.Invoke();
            foreach (var magnetable in new List<IMagnetable>(magnetedObjects))
                RemoveMagnetableObject(magnetable);
            magnetedObjects.Clear();
            orbitAngles.Clear();
        }
    }

    public void SetCustomMagnetData(MagnetData data)
    {
        customMagnetData = data;
        useCustomMagnetData = data != null;

        if (this.TryGetComponent(out MagneterVisual magneterVisual))
            magneterVisual.UpdateSizeRing();
    }

    public void UseMagneterManagerData() => useCustomMagnetData = false;

    private void OnDestroy()
    {
        if (detectionTimer != null && !detectionTimer.isDone)
            detectionTimer.Cancel();

        foreach (var magnetable in new List<IMagnetable>(magnetedObjects))
            RemoveMagnetableObject(magnetable);

        magnetedObjects.Clear();
        orbitAngles.Clear();
        _componentCache.Clear();
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = isActive ? Color.cyan : Color.gray;
        Gizmos.DrawWireSphere(transform.position, AttractionRadius);

        Gizmos.color = isActive ? Color.yellow : Color.gray;
        Gizmos.DrawWireSphere(transform.position, OrbitRadius);

        if (isActive)
        {
            Gizmos.color = Color.green;
            foreach (var magnetable in magnetedObjects)
                if (MagnetUtility.IsValidMagnetable(magnetable))
                    Gizmos.DrawLine(transform.position, magnetable.Transform.position);
        }
    }
}