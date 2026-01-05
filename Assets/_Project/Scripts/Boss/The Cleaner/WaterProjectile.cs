using UnityEngine;
public class WaterProjectile : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CurvedFlightMover flightMover;

    [Header("Erasure Settings")]
    [SerializeField, Range(0f,20f)] private float ErasureScale = 2f;

    private Vector3 targetPosition;
    private BossCleaner boss;

    public void Initialize(BossCleaner boss, Vector3 targetPosition)
    {
        this.boss = boss;
        this.targetPosition = targetPosition;

        flightMover.OnReachTarget += OnReachTargetPosition;
        flightMover.StartFlight(targetPosition);
    }

    private void OnDestroy()
    {
        flightMover.OnReachTarget -= OnReachTargetPosition;
    }

    private void OnReachTargetPosition()
    {
        Debug.Log("WaterProjectile reached target position, erasing paint.");
        ErasePaintInArea();
        Destroy(gameObject);
    }

    private void ErasePaintInArea()
    {
        Eraser.Instance.EraseAt(targetPosition, ErasureScale);
        boss.SpawnHealingPaint(transform.position);
    }
}
