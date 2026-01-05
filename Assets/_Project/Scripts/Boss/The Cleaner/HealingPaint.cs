using UnityEngine;

public class HealingPaint : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CurvedFlightMover flightMover;
    [SerializeField] private SimpleHealer healer;

    private IHealable target;
    private Transform targetTransform;

    private void Awake()
    {
        if (flightMover == null)
            flightMover = GetComponent<CurvedFlightMover>();

        if (healer == null)
            healer = GetComponent<SimpleHealer>();
    }

    public void Initialize(IHealable target, Vector3 startPosition, float healAmount, Transform targetTransform)
    {
        this.target = target;
        this.targetTransform = targetTransform;
        transform.position = startPosition;

        if (healer != null)
            healer.SetHealAmount(healAmount);

        flightMover.OnReachTarget += OnReachBoss;
        flightMover.StartFlight(targetTransform.position);
    }

    private void OnDestroy()
    {
        flightMover.OnReachTarget -= OnReachBoss;
    }
    private void OnReachBoss()
    {
        if (healer != null && target != null)
        {
            healer.PerformHeal(target);
        }

        Destroy(gameObject);
    }
}
