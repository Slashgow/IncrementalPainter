using inkolorgames;
using UnityEngine;
public class WaterProjectile : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CurvedFlightMover flightMover;

    [Header("Erasure Settings")]
    [SerializeField, Range(0f,20f)] private float ErasureScale = 2f;

    private Vector3 targetPosition;
    private BossCleaner boss;
    private PoolingSystem pool;

    public void Initialize(PoolingSystem pool, BossCleaner boss, Vector3 targetPosition)
    {
        this.pool = pool;
        this.boss = boss;
        this.targetPosition = targetPosition;

        flightMover.OnReachTarget += OnReachTargetPosition;
        flightMover.StartFlight(targetPosition);
    }

    private void OnDisable()
    {
        flightMover.OnReachTarget -= OnReachTargetPosition;
    }

    private void OnReachTargetPosition()
    {
        Debug.Log("WaterProjectile reached target position, erasing paint.");
        ErasePaintInArea();
        pool.AddToPool(gameObject);
    }

    private void ErasePaintInArea()
    {
        Eraser.Instance.EraseAt(targetPosition, ErasureScale);
        boss.SpawnHealingPaint(transform.position);
    }
}
