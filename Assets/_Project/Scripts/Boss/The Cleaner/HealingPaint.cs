using UnityEngine;

public class HealingPaint : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CurvedFlightMover flightMover;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private SimpleHealer healer;

    private IHealable target;

    private void Awake()
    {
        if (flightMover == null)
            flightMover = GetComponent<CurvedFlightMover>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (healer == null)
            healer = GetComponent<SimpleHealer>();
    }

    public void Initialize(IHealable target, Vector3 startPosition, Color paintColor, float healAmount)
    {
        this.target = target;
        transform.position = startPosition;

        if (spriteRenderer != null)
            spriteRenderer.color = paintColor;

        if (healer != null)
            healer.SetHealAmount(healAmount);

        flightMover.OnReachTarget += OnReachBoss;
        //flightMover.StartFlight(target.Transform.position);
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
