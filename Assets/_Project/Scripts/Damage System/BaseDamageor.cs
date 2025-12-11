using System;
using UnityEngine;
using UnityEngine.Events;

public abstract class BaseDamageor : MonoBehaviour, IDamageor
{
    [Header("Paramètres de dégâts")]
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> damageSkillDataPerLevel;
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> criticalDamageMultiplierSkillDataPerLevel;
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> criticalHitLuckSkillDataPerLevel;
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> damageRadiusSkillDataPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> DamageSkillDataPerLevel => damageSkillDataPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> CriticalDamageMultiplierSkillDataPerLevel => criticalDamageMultiplierSkillDataPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> CriticalHitLuckSkillDataPerLevel => criticalHitLuckSkillDataPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> DamageRadiusSkillDataPerLevel => damageRadiusSkillDataPerLevel;

    [SerializeField] protected LayerMask damageableLayers = ~0;
    public float Damage => damageSkillDataPerLevel.GetCurrentLevelData();
    public float CriticalDamageMultiplier => criticalDamageMultiplierSkillDataPerLevel.GetCurrentLevelData();
    public float CriticalHitLuck => criticalHitLuckSkillDataPerLevel.GetCurrentLevelData();
    public float DamageRadius => damageRadiusSkillDataPerLevel.GetCurrentLevelData();

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
            if (damageable != null)
            {
                anyDamageDealt = true;
                float damageAmount = CalculateDamage(out bool isCritical);
                OnAnyDamageorAttack?.Invoke(damageAmount, collider.transform.position, isCritical);
                NotityDamage(damageAmount);
                damageable.TakeDamage(Damage);
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
