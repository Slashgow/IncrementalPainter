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

    [Header("Collection")]
    [SerializeField] private float collectDistance = 0.5f;

    [Header("Settings")]
    [SerializeField] private bool isActive = true;

    public Transform Transform => transform;
    public float VacuumForce => vacuumForcePerLevel.GetCurrentLevelData();
    public float VacuumRange => vacuumRangePerLevel.GetCurrentLevelData();
    public float CollectDistance => collectDistance;
    public bool IsActive => isActive;

    private HashSet<IVacuumable> vacuumedObjects = new HashSet<IVacuumable>();
    private Timer detectionTimer;

    public UnityEvent OnVacuumerActivated;
    public UnityEvent OnVacuumerDeactivated;
    public UnityEvent<IVacuumable> OnVacuumableCollected;

    public static event Action<IVacuumer, IVacuumable> OnAnyVacuumerCollect;

    private void Start()
    {
        StartDetectionTimer();
    }

    private void StartDetectionTimer()
    {
        detectionTimer = Timer.Register(detectionInterval, onComplete: DetectVacuumableObjects, isLooped: true, useRealTime: false);
    }

    private void Update()
    {
        if (!isActive)
            return;

        ApplyVacuumMovement();
    }

    private void DetectVacuumableObjects()
    {
        if (!isActive)
            return;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, VacuumRange, vacuumableLayers);

        HashSet<IVacuumable> objectsInRange = new HashSet<IVacuumable>();

        foreach (var collider in colliders)
        {
            var vacuumable = collider.GetComponentInParent<IVacuumable>();
            if (vacuumable != null && VacuumUtility.IsValidVacuumable(vacuumable))
            {
                objectsInRange.Add(vacuumable);

                if (!vacuumedObjects.Contains(vacuumable))
                {
                    vacuumedObjects.Add(vacuumable);
                }
            }
        }

        vacuumedObjects.RemoveWhere(vacuumable =>
        {
            if (!VacuumUtility.IsValidVacuumable(vacuumable) || !objectsInRange.Contains(vacuumable))
            {
                return true;
            }
            return false;
        });
    }

    private void ApplyVacuumMovement()
    {
        List<IVacuumable> vacuumablesCopy = new List<IVacuumable>(vacuumedObjects);

        foreach (var vacuumable in vacuumablesCopy)
        {
            if (!VacuumUtility.IsValidVacuumable(vacuumable))
            {
                vacuumedObjects.Remove(vacuumable);
                continue;
            }

            if (!vacuumable.CanBeVacuumed)
                continue;

            Vector3 currentPos = vacuumable.Transform.position;
            float distance = Vector3.Distance(currentPos, transform.position);

            if (distance <= CollectDistance)
            {
                CollectVacuumable(vacuumable);
                continue;
            }

            Vector3 movement = VacuumUtility.CalculateVacuumMovement(currentPos, transform.position, VacuumForce, vacuumable.VacuumResistance, Time.deltaTime);
            vacuumable.ApplyVacuumMovement(movement);
        }
    }

    public void CollectVacuumable(IVacuumable vacuumable)
    {
        if (!VacuumUtility.IsValidVacuumable(vacuumable))
            return;

        vacuumedObjects.Remove(vacuumable);

        vacuumable.OnVacuumedCollect(this);

        OnVacuumableCollected?.Invoke(vacuumable);
        OnAnyVacuumerCollect?.Invoke(this, vacuumable);
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

            // Clear all vacuumed objects
            vacuumedObjects.Clear();
        }
    }

    private void OnDestroy()
    {
        detectionTimer?.Cancel();
        vacuumedObjects.Clear();
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        // Draw vacuum range
        Gizmos.color = isActive ? new Color(0f, 1f, 1f, 0.3f) : Color.gray;
        Gizmos.DrawWireSphere(transform.position, VacuumRange);

        // Draw collect distance
        Gizmos.color = isActive ? new Color(1f, 1f, 0f, 0.5f) : Color.gray;
        Gizmos.DrawWireSphere(transform.position, CollectDistance);

        // Draw lines to vacuumed objects
        if (isActive)
        {
            Gizmos.color = Color.cyan;
            foreach (var vacuumable in vacuumedObjects)
            {
                if (VacuumUtility.IsValidVacuumable(vacuumable))
                {
                    Gizmos.DrawLine(transform.position, vacuumable.Transform.position);
                }
            }
        }
    }
}