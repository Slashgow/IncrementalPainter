using System;
using UnityEngine;
using UnityEngine.Events;

public class SimpleVacuumable : MonoBehaviour, IVacuumable
{
    [Header("Vacuum Settings")]
    [SerializeField, Range(0f, 1f)] private float vacuumResistance = 0f;
    [SerializeField] private bool canBeVacuumed = true;

    [Header("Movement Settings")]
    [SerializeField] private bool smoothMovement = true;
    [SerializeField] private float smoothSpeed = 10f;

    private IVacuumer currentVacuumer;
    private Vector3 targetPosition;

    public Transform Transform => transform;
    public bool CanBeVacuumed => canBeVacuumed;
    public float VacuumResistance => vacuumResistance;
    public bool IsBeingVacuumed => currentVacuumer != null && currentVacuumer.IsActive;

    public UnityEvent<IVacuumer> OnVacuumCollectedUnityEvent;
    public event Action<IVacuumer> OnVacuumCollected;

    public static event Action<IVacuumable, IVacuumer> OnAnyVacuumableCollected;

    private void OnEnable()
    {
        if(targetPosition == Vector3.zero)
            Initialize(transform.position);
    }

    public void Initialize(Vector3 targetPosition) => this.targetPosition = targetPosition;

    public void ApplyVacuumMovement(Vector3 movement)
    {
        if (!canBeVacuumed)
            return;

        targetPosition += movement;

        if (smoothMovement)
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
        else
            transform.position = targetPosition;
    }

    public void OnVacuumedCollect(IVacuumer vacuumer)
    {
        currentVacuumer = vacuumer;

        OnVacuumCollected?.Invoke(vacuumer);
        OnVacuumCollectedUnityEvent?.Invoke(vacuumer);
        OnAnyVacuumableCollected?.Invoke(this, vacuumer);
    }

    public void SetVacuumer(IVacuumer vacuumer) => currentVacuumer = vacuumer;
    public void ClearVacuumer() => currentVacuumer = null;
}