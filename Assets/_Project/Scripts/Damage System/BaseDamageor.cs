using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public abstract class BaseDamageor : MonoBehaviour, IDamageor
{
    [Header("Paramètres de dégâts")]
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> damageSkillDataPerLevel;
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> criticalDamageMultiplierSkillDataPerLevel;
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> criticalHitLuckSkillDataPerLevel;
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> damageRadiusSkillDataPerLevel;

    [Header("Shield Breaking Upgrades")]
    [SerializeField] private bool useShieldBreakingUpgrades = false;
    [SerializeField, ShowIf("useShieldBreakingUpgrades")] private SkillDataPerLevelOfType<FunctionAffine> shieldPenetrationSkillDataPerLevel; // 0-1 (0-100%)
    [SerializeField, ShowIf("useShieldBreakingUpgrades")] private SkillDataPerLevelOfType<FunctionAffine> shieldBreakBonusSkillDataPerLevel; // Flat bonus damage vs shields
    [SerializeField, ShowIf("useShieldBreakingUpgrades")] private SkillDataPerLevelOfType<FunctionAffine> shieldShredderSkillDataPerLevel; // 0-1 armor reduction
    [SerializeField, ShowIf("useShieldBreakingUpgrades")] private SkillDataPerLevelOfType<FunctionAffine> overloadDamageMultiplierSkillDataPerLevel; // x1.2
    [SerializeField, ShowIf("useShieldBreakingUpgrades")] private SkillDataPerLevelOfType<FunctionAffine> shieldBypassLuckSkillDataPerLevel; // 0-1 chance to ignore shield
    public SkillDataPerLevelOfType<FunctionAffine> DamageSkillDataPerLevel => damageSkillDataPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> CriticalDamageMultiplierSkillDataPerLevel => criticalDamageMultiplierSkillDataPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> CriticalHitLuckSkillDataPerLevel => criticalHitLuckSkillDataPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> DamageRadiusSkillDataPerLevel => damageRadiusSkillDataPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> ShieldPenetrationSkillDataPerLevel => shieldPenetrationSkillDataPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> ShieldBreakBonusSkillDataPerLevel => shieldBreakBonusSkillDataPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> ShieldShredderSkillDataPerLevel => shieldShredderSkillDataPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> OverloadDamageMultiplierSkillDataPerLevel => overloadDamageMultiplierSkillDataPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> ShieldBypassLuckSkillDataPerLevel => shieldBypassLuckSkillDataPerLevel;

    [SerializeField] protected LayerMask damageableLayers = ~0;
    public LayerMask DamageableLayers => damageableLayers;
    public float Damage => damageSkillDataPerLevel.GetCurrentLevelData();
    public float CriticalDamageMultiplier => criticalDamageMultiplierSkillDataPerLevel.GetCurrentLevelData();
    public float CriticalHitLuck => criticalHitLuckSkillDataPerLevel.GetCurrentLevelData();
    public virtual float DamageRadius => damageRadiusSkillDataPerLevel.GetCurrentLevelData();
    public float ShieldPenetration => shieldPenetrationSkillDataPerLevel.GetCurrentLevelData();
    public float ShieldBreakBonus => shieldBreakBonusSkillDataPerLevel.GetCurrentLevelData();
    public float ShieldShredder => shieldShredderSkillDataPerLevel.GetCurrentLevelData();
    public float OverloadDamageMultiplier => overloadDamageMultiplierSkillDataPerLevel.GetCurrentLevelData();
    public float ShieldBypassLuck => shieldBypassLuckSkillDataPerLevel.GetCurrentLevelData();

    public UnityEvent OnAttackOnce;
    public static event Action<float, Vector3, bool> OnAnyDamageorAttack;

    public void TryDamage(Vector3 clickPosition)
    {
        TryDamageFromWorldPoint(clickPosition);
    }

    private void TryDamageFromWorldPoint(Vector3 worldPos)
    {
        Vector2 worldPosition2D = new Vector2(worldPos.x, worldPos.y);
        Collider2D[] colliders2D = Physics2D.OverlapCircleAll(worldPosition2D, DamageRadius, damageableLayers.value);
        bool anyDamageDealt = false;

        foreach (var collider in colliders2D)
        {
            var damageable = collider.GetComponentInParent<IDamageable>();
            if (damageable == null)
                continue;

            float damageAmount = CalculateDamage(out bool isCritical);

            bool bypassedShield = useShieldBreakingUpgrades ? LuckUtility.RollLuck01(ShieldBypassLuck) : false;

            var shieldable = collider.GetComponentInParent<IShieldable>();
            float remainingDamage = damageAmount;

            if (shieldable != null && shieldable.IsShieldActive && !bypassedShield)
            {
                float overflowDamage = useShieldBreakingUpgrades ? 
                    shieldable.DamageShield(damageAmount, ShieldPenetration, ShieldBreakBonus, ShieldShredder) :
                    shieldable.DamageShield(damageAmount);

                remainingDamage = overflowDamage * (useShieldBreakingUpgrades ? OverloadDamageMultiplier : 1f);

                anyDamageDealt = true;
                OnAnyDamageorAttack?.Invoke(damageAmount, collider.transform.position, isCritical);
                NotityDamage(damageAmount);
            }
            else
            {
                remainingDamage = damageAmount;
            }

            if (remainingDamage > 0f)
            {
                anyDamageDealt = true;

                // Only invoke attack event if we didn't already (from shield damage)
                if (shieldable == null || !shieldable.IsShieldActive || bypassedShield)
                {
                    OnAnyDamageorAttack?.Invoke(remainingDamage, collider.transform.position, isCritical);
                    NotityDamage(remainingDamage);
                }

                damageable.TakeDamage(remainingDamage);
                
            }
        }
        if (anyDamageDealt)
            OnAttackOnce?.Invoke();
    }
    private float CalculateDamage(out bool isCritical)
    {
        isCritical = LuckUtility.RollLuck(CriticalHitLuck);
        return isCritical ? Damage * CriticalDamageMultiplier : Damage;
    }

    public abstract void NotityDamage(float damage);
}
