using System;
using System.Collections.Generic;
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
    [SerializeField, ShowIf("useShieldBreakingUpgrades")] private SkillDataPerLevelOfType<FunctionAffine> shieldPenetrationSkillDataPerLevel;
    [SerializeField, ShowIf("useShieldBreakingUpgrades")] private SkillDataPerLevelOfType<FunctionAffine> shieldBreakBonusSkillDataPerLevel;
    [SerializeField, ShowIf("useShieldBreakingUpgrades")] private SkillDataPerLevelOfType<FunctionAffine> shieldShredderSkillDataPerLevel;
    [SerializeField, ShowIf("useShieldBreakingUpgrades")] private SkillDataPerLevelOfType<FunctionAffine> overloadDamageMultiplierSkillDataPerLevel;
    [SerializeField, ShowIf("useShieldBreakingUpgrades")] private SkillDataPerLevelOfType<FunctionAffine> shieldBypassLuckSkillDataPerLevel;

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

    [Header("Performance")]
    [SerializeField, Min(1)] private int damageablesPerFrame = 15;

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
    public static event Action<float> OnAnyDamageaorAttackOnce;

    private float cumulateRawDamageOneHit;
    // ?? Damage queue ?????????????????????????????????????????????????????????

    private struct PendingDamage
    {
        public Collider2D Collider;
        public float DamageAmount;
        public bool IsCritical;
    }

    private readonly Queue<PendingDamage> _pendingDamages = new();
    private bool _hasPendingAttack; // tracks whether OnAttackOnce should fire

    // ?? Public API ???????????????????????????????????????????????????????????

    public void TryDamage(Vector3 clickPosition)
    {
        TryDamageFromWorldPoint(clickPosition);
    }

    // ?? Detection — runs immediately, enqueues results ???????????????????????

    private void TryDamageFromWorldPoint(Vector3 worldPos)
    {
        cumulateRawDamageOneHit = 0;
        Vector2 worldPosition2D = new Vector2(worldPos.x, worldPos.y);
        Collider2D[] colliders2D = Physics2D.OverlapCircleAll(worldPosition2D, DamageRadius, damageableLayers.value);

        foreach (var collider in colliders2D)
        {
            // Quick null check before enqueuing — skip if no damageable at all
            if (collider.GetComponentInParent<IDamageable>() == null)
                continue;

            float damageAmount = CalculateDamage(out bool isCritical);
            cumulateRawDamageOneHit += damageAmount;

            _pendingDamages.Enqueue(new PendingDamage
            {
                Collider = collider,
                DamageAmount = damageAmount,
                IsCritical = isCritical,
            });
        }
    }

    // ?? Processing — spread over frames ??????????????????????????????????????

    private void Update()
    {
        if (_pendingDamages.Count == 0) return;

        int toProcess = Mathf.Min(damageablesPerFrame, _pendingDamages.Count);

        for (int i = 0; i < toProcess; i++)
        {
            ProcessDamage(_pendingDamages.Dequeue());
        }

        // Fire OnAttackOnce after the last batch of this attack wave
        if (_hasPendingAttack && _pendingDamages.Count == 0)
        {
            OnAttackOnce?.Invoke();
            OnAnyDamageaorAttackOnce?.Invoke(cumulateRawDamageOneHit);
            _hasPendingAttack = false;
        }
    }

    private void ProcessDamage(PendingDamage pending)
    {
        Collider2D collider = pending.Collider;

        // Object may have been destroyed between enqueue and processing
        if (collider == null) return;

        var damageable = collider.GetComponentInParent<IDamageable>();
        if (damageable == null) return;

        float damageAmount = pending.DamageAmount;
        bool isCritical = pending.IsCritical;

        bool bypassedShield = useShieldBreakingUpgrades && LuckUtility.RollLuck01(ShieldBypassLuck);

        var shieldable = collider.GetComponentInParent<IShieldable>();
        float remainingDamage = damageAmount;

        if (shieldable != null && shieldable.IsShieldActive && !bypassedShield)
        {
            float overflowDamage = useShieldBreakingUpgrades
                ? shieldable.DamageShield(damageAmount, ShieldPenetration, ShieldBreakBonus, ShieldShredder)
                : shieldable.DamageShield(damageAmount);

            remainingDamage = overflowDamage * (useShieldBreakingUpgrades ? OverloadDamageMultiplier : 1f);

            OnAnyDamageorAttack?.Invoke(damageAmount, collider.transform.position, isCritical);
            NotityDamage(damageAmount);
        }
        else
        {
            remainingDamage = damageAmount;
        }

        if (remainingDamage > 0f)
        {
            if (shieldable == null || !shieldable.IsShieldActive || bypassedShield)
            {
                OnAnyDamageorAttack?.Invoke(remainingDamage, collider.transform.position, isCritical);
                NotityDamage(remainingDamage);
            }

            damageable.TakeDamage(remainingDamage);
        }

        _hasPendingAttack = true;
    }

    private void OnDisable()
    {
        _pendingDamages.Clear();
        _hasPendingAttack = false;
    }

    // ?? Helpers ??????????????????????????????????????????????????????????????

    private float CalculateDamage(out bool isCritical)
    {
        isCritical = LuckUtility.RollLuck(CriticalHitLuck);
        return isCritical ? Damage * CriticalDamageMultiplier : Damage;
    }

    public abstract void NotityDamage(float damage);
}