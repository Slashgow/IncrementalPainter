using System;
using System.Collections.Generic;
using inkolorgames;
using UnityEngine;

public class TurretDamageor : BaseDamageor
{
    [Header("Turret Parameters")]
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> attackRangeSkillDataPerLevel;
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> attackSpeedSkillDataPerLevel; // Attacks per second
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> projectilesPerAttackSkillDataPerLevel;

    [Header("Projectile")]
    [SerializeField] private PoolingSystem projectilePrefabPool;

    public SkillDataPerLevelOfType<FunctionAffine> AttackRangeSkillDataPerLevel => attackRangeSkillDataPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> AttackSpeedSkillDataPerLevel => attackSpeedSkillDataPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> ProjectilesPerAttackSkillDataPerLevel => projectilesPerAttackSkillDataPerLevel;

    public float AttackRange => attackRangeSkillDataPerLevel.GetCurrentLevelData();
    public float AttackSpeed => attackSpeedSkillDataPerLevel.GetCurrentLevelData();
    public int ProjectilesPerAttack => Mathf.RoundToInt(projectilesPerAttackSkillDataPerLevel.GetCurrentLevelData());

    public float AttackInterval => AttackSpeed > 0f ? 1f / AttackSpeed : float.MaxValue;

    public static event Action<float> OnTurretDamage;
    public event Action OnFireOnce;
    public override void NotityDamage(float damage) => OnTurretDamage?.Invoke(damage);

    public void TryAttack(Vector3 position, Vector3 projectileSpawnPoint)
    {
        List<IDamageable> targets = GetTargetsInRange(position);
        if (targets.Count > 0)
        {
            OnFireOnce?.Invoke();
            FireAtTargets(targets, projectileSpawnPoint);
        }
            
    }

    private void FireAtTargets(List<IDamageable> targets, Vector3 projectileSpawnPoint)
    {
        for (int i = 0; i < ProjectilesPerAttack; i++)
        {
            IDamageable target = targets[i % targets.Count];

            if (target is MonoBehaviour mb)
            {
                GameObject projectileObj = projectilePrefabPool.GetPrefabFromPool(projectileSpawnPoint);
                TurretProjectile projectile = projectileObj.GetComponent<TurretProjectile>();
                projectile.Initialize(this, target, mb.transform, projectilePrefabPool);
            }
        }
    }

    public List<IDamageable> GetTargetsInRange(Vector3 position)
    {
        List<IDamageable> targets = new List<IDamageable>();
        Collider2D[] hits = Physics2D.OverlapCircleAll(position, AttackRange, damageableLayers);

        foreach (var hit in hits)
        {
            var damageable = hit.GetComponentInParent<IDamageable>();
            if (damageable != null && !targets.Contains(damageable))
                targets.Add(damageable);
        }

        return targets;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackRangeSkillDataPerLevel == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
    }
}
