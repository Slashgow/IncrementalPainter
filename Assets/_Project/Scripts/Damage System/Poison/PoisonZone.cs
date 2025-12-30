using System;
using System.Collections.Generic;
using UnityEngine;
using UnityTimer;

public class PoisonZone : MonoBehaviour
{
    private PoisonDamageor damageor;
    private PoisonZoneData poisonZoneData;

    private Timer durationTimer;
    private Timer tickTimer;
    private HashSet<IDamageable> damageablesInZone = new HashSet<IDamageable>();

    [Header("Visual Settings")]
    [SerializeField] private ParticleSystem poisonParticles, glowPoisonParticle;

    public static event Action<float, Vector3, bool> OnAnyPoisonAttack;

    private bool canAttack = false;
    public void Initialize(PoisonDamageor damageor, PoisonZoneData poisonZoneData)
    {
        this.damageor = damageor;
        this.poisonZoneData = poisonZoneData;
        canAttack = true;
        SetupVisuals();
        StartDamageOverTime();
    }

    private void SetupVisuals()
    {
        transform.localScale = Vector3.one * poisonZoneData.Radius * 2f;

        if (poisonParticles != null)
        {
            var main = poisonParticles.main;
            main.startColor = poisonZoneData.ZoneColor;

            var glowMain = glowPoisonParticle.main;
            glowMain.startColor = poisonZoneData.ZoneColor;

            poisonParticles.Play();
            //glowPoisonParticle.Play();
        }
    }

    private void StartDamageOverTime()
    {
        durationTimer = Timer.Register(poisonZoneData.Duration, onComplete: () => DestroyZone(), useRealTime: false);
        tickTimer = Timer.Register(poisonZoneData.TickInterval, onComplete: () => DamageAllInZone(), isLooped: true, useRealTime: false);
    }

    private void DamageAllInZone()
    {
        if(!canAttack)
            return;

        damageablesInZone.Clear();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, poisonZoneData.Radius, poisonZoneData.DamageableLayers);

        foreach (var collider in colliders)
        {
            if(collider.TryGetComponent<IDamageable>(out var damageable))
            {
                float damage = CalculateDamage(out bool isCritical);
                damageor.NotityDamage(damage);
                OnAnyPoisonAttack?.Invoke(damage, collider.transform.position, isCritical);
                damageable.TakeDamage(damage);
            }
        }
    }

    private float CalculateDamage(out bool isCritical)
    {
        isCritical = LuckUtility.RollLuck(poisonZoneData.CriticalHitLuck);
        return isCritical ? poisonZoneData.DamagePerTick * poisonZoneData.CriticalDamageMultiplier : poisonZoneData.DamagePerTick;
    }

    private void DestroyZone()
    {
        canAttack = false;

        if (poisonParticles != null)
            poisonParticles.Stop();

        durationTimer?.Cancel();
        tickTimer?.Cancel();

        //Destroy(gameObject);
    }

    private void OnDestroy()
    {
        durationTimer?.Cancel();
        tickTimer?.Cancel();
        damageablesInZone.Clear();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, poisonZoneData.Radius);
    }
}