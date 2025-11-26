using UnityEngine;


public class SimpleDamageor : MonoBehaviour, IDamageor
{
    [Header("Références")]
    [SerializeField] private AutoClicker autoClicker;

    [Header("Paramètres de dégâts")]
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> damageSkillDataPerLevel;
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> criticalDamageMultiplierSkillDataPerLevel;
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> criticalHitLuckSkillDataPerLevel;
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> damageRadiusSkillDataPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> DamageSkillDataPerLevel => damageSkillDataPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> CriticalDamageMultiplierSkillDataPerLevel => criticalDamageMultiplierSkillDataPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> CriticalHitLuckSkillDataPerLevel => criticalHitLuckSkillDataPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> DamageRadiusSkillDataPerLevel => damageRadiusSkillDataPerLevel;

    [SerializeField] private LayerMask damageableLayers = ~0;
    public float Damage => damageSkillDataPerLevel.GetCurrentLevelData();
    public float CriticalDamageMultiplier => criticalDamageMultiplierSkillDataPerLevel.GetCurrentLevelData();
    public float CriticalHitLuck => criticalHitLuckSkillDataPerLevel.GetCurrentLevelData();
    public float DamageRadius => damageRadiusSkillDataPerLevel.GetCurrentLevelData();

    private void OnEnable()
    {
        damageSkillDataPerLevel.Initialize();

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
        Collider2D[] colliders2D = Physics2D.OverlapCircleAll(worldPosition2D, DamageRadius, damageableLayers.value);
        foreach (var collider in colliders2D)
        {
            var damageable = collider.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                float damageAmount = CalculateDamage(out bool isCritical);
                damageable.TakeDamage(Damage);
            }
        }
    }
    private float CalculateDamage(out bool isCritical)
    {
        isCritical = LuckUtility.RollLuck(CriticalHitLuck);
        return isCritical ? Damage * CriticalDamageMultiplier : Damage;
    }
}