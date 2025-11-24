using UnityEngine;

public class SimpleDamageor : MonoBehaviour, IDamageor
{
    [Header("Références")]
    [SerializeField] private AutoClicker autoClicker;

    [Header("Paramètres de dégâts")]
    [SerializeField] private float damage = 1f;
    [SerializeField] private float damageRadius = 0.1f;
    [SerializeField] private LayerMask damageableLayers = ~0;
    [SerializeField] private float rayDistance = 100f;
    public float Damage => damage;
    public float DamageRadius => damageRadius;

    private void OnEnable()
    {
        if (autoClicker != null)
            autoClicker.OnClick += HandleClick;
    }

    private void OnDisable()
    {
        if (autoClicker != null)
            autoClicker.OnClick -= HandleClick;
    }

    private void HandleClick(Vector3 clickPosition) => TryDamage(clickPosition);

    public void TryDamage(Vector3 clickPosition)
    {
        TryDamageFromWorldPoint(clickPosition);
    }

    private void TryDamageFromWorldPoint(Vector3 worldPos)
    {
        Vector2 worldPosition2D = new Vector2(worldPos.x, worldPos.y);
        Collider2D[] colliders2D = Physics2D.OverlapCircleAll(worldPosition2D, damageRadius, damageableLayers.value);
        foreach (var collider in colliders2D)
        {
            var damageable = collider.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
        }
    }
}