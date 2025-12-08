using System;
using UnityEngine;
using UnityEngine.Events;

public class SimpleMagnetable : MonoBehaviour, IMagnetable
{
    [Header("Magnet Settings")]
    [SerializeField, Range(0f, 1f)] private float magnetResistance = 0f;
    [SerializeField] private bool canBeMagneted = true;

    [Header("Movement Settings")]
    [SerializeField] private bool smoothMovement = true;
    [SerializeField] private float smoothSpeed = 10f;

    private IMagneter currentMagneter;
    private Vector3 targetPosition;

    public Transform Transform => transform;
    public bool IsBeingMagneted => currentMagneter != null && currentMagneter.IsActive;
    public float MagnetResistance => magnetResistance;

    public UnityEvent<IMagneter> OnMagnetEnteredUnityEvent;
    public UnityEvent<IMagneter> OnMagnetExitedUnityEvent;

    public event Action<IMagneter> OnMagnetEntered;
    public event Action<IMagneter> OnMagnetExited;

    private void Awake()
    {
        targetPosition = transform.position;
    }

    public void ApplyMagneticMovement(Vector3 movement)
    {
        if (!canBeMagneted)
            return;

        targetPosition += movement;

        if (smoothMovement)
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
        else
            transform.position = targetPosition;
    }

    public void OnMagnetEnter(IMagneter magneter)
    {
        if (!canBeMagneted)
            return;

        currentMagneter = magneter;
        targetPosition = transform.position;
        OnMagnetEntered?.Invoke(magneter);
        OnMagnetEnteredUnityEvent?.Invoke(magneter);
    }

    public void OnMagnetExit(IMagneter magneter)
    {
        if (currentMagneter == magneter)
        {
            currentMagneter = null;
            OnMagnetExited?.Invoke(magneter);
            OnMagnetExitedUnityEvent?.Invoke(magneter);
        }
    }

    private void OnDisable()
    {
        if (currentMagneter != null)
        {
            currentMagneter.RemoveMagnetableObject(this);
            currentMagneter = null;
        }
    }
}
