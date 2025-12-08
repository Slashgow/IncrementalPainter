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

    [Header("Settings")]
    [SerializeField] private bool isActive = true;

    public Transform Transform => transform;
    public float AttractionForce => MagneterManager.Instance.AttractionForcePerLevel.GetCurrentLevelData();
    public float AttractionRadius => MagneterManager.Instance.AttractionRadiusPerLevel.GetCurrentLevelData();
    public float OrbitRadius => MagneterManager.Instance.OrbitRadiusPerLevel.GetCurrentLevelData();
    public float OrbitSpeed => MagneterManager.Instance.OrbitSpeedPerLevel.GetCurrentLevelData();
    public bool IsActive => isActive;

    private HashSet<IMagnetable> magnetedObjects = new HashSet<IMagnetable>();
    private Dictionary<IMagnetable, float> orbitAngles = new Dictionary<IMagnetable, float>();
    private Timer detectionTimer;

    public UnityEvent OnMagnetActivated;
    public UnityEvent OnMagnetDeactivated;
    public static event Action<IMagneter, IMagnetable> OnAnyMagnetAttract;
    public static event Action<IMagneter, IMagnetable> OnAnyMagnetRelease;

    private void Start()
    {
        StartDetectionTimer();
    }

    private void StartDetectionTimer()
    {
        detectionTimer = Timer.Register(detectionInterval, onComplete: DetectMagnetableObjects, isLooped: true, useRealTime: false);
    }

    private void Update()
    {
        if (!isActive)
            return;

        ApplyMagneticMovement();
    }

    private void DetectMagnetableObjects()
    {
        if (!isActive)
            return;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, AttractionRadius, magnetableLayers);

        HashSet<IMagnetable> objectsInRange = new HashSet<IMagnetable>();
        foreach (var collider in colliders)
        {
            var magnetable = collider.GetComponentInParent<IMagnetable>();
            if (magnetable != null && MagnetUtility.IsValidMagnetable(magnetable))
            {
                objectsInRange.Add(magnetable);

                if (!magnetedObjects.Contains(magnetable))
                {
                    AddMagnetableObject(magnetable);
                }
            }
        }

        magnetedObjects.RemoveWhere(magnetable =>
        {
            if (!MagnetUtility.IsValidMagnetable(magnetable) || !objectsInRange.Contains(magnetable))
            {
                RemoveMagnetableObject(magnetable);
                return true;
            }
            return false;
        });
    }

    private void ApplyMagneticMovement()
    {
        List<IMagnetable> magnetablesCopy = new List<IMagnetable>(magnetedObjects);

        foreach (var magnetable in magnetablesCopy)
        {
            if (!MagnetUtility.IsValidMagnetable(magnetable))
            {
                RemoveMagnetableObject(magnetable);
                continue;
            }

            Vector3 currentPos = magnetable.Transform.position;
            float distance = Vector3.Distance(currentPos, transform.position);

            if (distance < 0.01f)
                continue;

            Vector3 movement;

            if (distance > OrbitRadius)
                movement = MagnetUtility.CalculatePullMovement(currentPos,transform.position,AttractionForce,magnetable.MagnetResistance,Time.deltaTime);
            else
                movement = CalculateOrbitMovement(magnetable, distance);

            magnetable.ApplyMagneticMovement(movement);
        }
    }

    private Vector3 CalculateOrbitMovement(IMagnetable magnetable, float currentDistance)
    {
        if (!orbitAngles.ContainsKey(magnetable))
        {
            Vector3 relativePos = magnetable.Transform.position - transform.position;
            orbitAngles[magnetable] = MagnetUtility.CalculateAngleFromPosition(relativePos);
        }

        float angleIncrement = MagnetUtility.CalculateOrbitAngleIncrement(OrbitSpeed, Time.deltaTime);
        orbitAngles[magnetable] += angleIncrement;

        return MagnetUtility.CalculateOrbitMovement(magnetable.Transform.position, transform.position, orbitAngles[magnetable], OrbitRadius, OrbitSpeed,
            AttractionForce, magnetable.MagnetResistance, Time.deltaTime);
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
            magnetable.OnMagnetExit(this);
            OnAnyMagnetRelease?.Invoke(this, magnetable);
        }
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

            // Release all magneted objects
            foreach (var magnetable in new List<IMagnetable>(magnetedObjects))
            {
                RemoveMagnetableObject(magnetable);
            }
            magnetedObjects.Clear();
            orbitAngles.Clear();
        }
    }

    private void OnDestroy()
    {
        if (detectionTimer != null && !detectionTimer.isDone)
            detectionTimer.Cancel();

        // Clean up all magneted objects
        foreach (var magnetable in new List<IMagnetable>(magnetedObjects))
        {
            RemoveMagnetableObject(magnetable);
        }
        magnetedObjects.Clear();
        orbitAngles.Clear();
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        // Draw attraction radius
        Gizmos.color = isActive ? Color.cyan : Color.gray;
        Gizmos.DrawWireSphere(transform.position, AttractionRadius);

        // Draw orbit radius
        Gizmos.color = isActive ? Color.yellow : Color.gray;
        Gizmos.DrawWireSphere(transform.position, OrbitRadius);

        // Draw lines to magneted objects
        if (isActive)
        {
            Gizmos.color = Color.green;
            foreach (var magnetable in magnetedObjects)
            {
                if (MagnetUtility.IsValidMagnetable(magnetable))
                {
                    Gizmos.DrawLine(transform.position, magnetable.Transform.position);
                }
            }
        }
    }
}