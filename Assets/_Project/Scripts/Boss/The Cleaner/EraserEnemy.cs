using System;
using inkolorgames;
using UnityEngine;
using UnityTimer;


public class EraserEnemy : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CurvedFlightMover flightMover;
    [SerializeField] private bool eraseOnImpact = true;

    [Header("Erasure Settings")]
    [SerializeField] private bool eraseContinuously = false;
    [SerializeField, Range(0f, 0.5f)] private float timeBetweenErasure = 0.1f; 
    [SerializeField, Range(0f,20f)] private float ErasureScale = 2f;

    private Vector3 targetPosition;
    private BossCleaner boss;
    private PoolingSystem pool;
    private Timer flightErasureTimer;
    public void Initialize(PoolingSystem pool, Vector3 targetPosition, BossCleaner boss = null)
    {
        this.pool = pool;
        this.boss = boss;
        this.targetPosition = targetPosition;

        if(eraseOnImpact && flightMover != null)
        {
            flightMover.OnReachTarget += OnReachTargetPosition;
            flightMover.StartFlight(targetPosition);
        }

        if(!eraseContinuously)
            return;

        flightErasureTimer = Timer.Register(timeBetweenErasure, onComplete: () => ErasePaintInArea(), isLooped: true, useRealTime: false);
    }

    protected virtual void OnDisable()
    {
        if(flightMover != null)
            flightMover.OnReachTarget -= OnReachTargetPosition;

        flightErasureTimer?.Cancel();
    }

    private void OnReachTargetPosition()
    {
        //Debug.Log("WaterProjectile reached target position, erasing paint.");
        ErasePaintInArea();
        boss.SpawnHealingPaint(transform.position);
        pool.AddToPool(gameObject);
    }

    protected void ErasePaintInArea() => Eraser.Instance.EraseAt(this.transform.position, ErasureScale);
}
