using System;
using System.Collections.Generic;
using inkolorgames;
using UnityEngine;
using UnityEngine.Events;
using UnityTimer;

public class BossTimeThief : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SimpleDamageable damageable;

    [Header("Time Steal Configuration")]
    [SerializeField, Range(0f,15f)] private float stealTimeInterval = 5f;
    [SerializeField, Range(0f,10f)] private float stealAmount = 3f;
    [SerializeField, Range(0f,50f)] private float healPerTimeStolen = 10f; // Heal amount = stolen time * this multiplier
    [SerializeField] private UnityEvent OnStealTime;

    [Header("Time Shard Drop Configuration")]
    [SerializeField, Range(0f,100f)] private float damageThreshold = 50f;
    [SerializeField, Range(0f, 10f)] private float damageWindow = 5f;
    [SerializeField] private PoolingSystem timeShardPool;
    [SerializeField, Range(0f, 10f)] private float dropRadius = 2f;
    [SerializeField, Range(0f, 10f)] private float timeRestorePerShard = 5f;

    [Header("Phase 2 Settings (Optional)")]
    [SerializeField, Range(0f, 1f)] private float phase2HealthThreshold = 0.5f;
    [SerializeField, Range(0f, 10f)] private float phase2StealMultiplier = 1.5f; // Increase steal amount in phase 2

    private Queue<(float time, float dmg)> damageHistory = new Queue<(float, float)>();
    private Timer stealTimer;
    private float lastHealth;
    private bool isPhase2 = false;
    private float currentStealAmount;

    private void OnEnable()
    {
        damageable.OnTakeDamage += HandleTakeDamage;
        LevelManager.OnMidLevel += EnterPhase2;
    }

    private void OnDisable()
    {
        damageable.OnTakeDamage -= HandleTakeDamage;
        LevelManager.OnMidLevel -= EnterPhase2;
    }

    private void Start()
    {
        lastHealth = damageable.MaxHealth;
        currentStealAmount = stealAmount;
        StartStealTimer();
    }

    private void StartStealTimer()
    {
        stealTimer = Timer.Register(stealTimeInterval, onComplete: StealTime, isLooped: true);
    }

    private void StealTime()
    {
        float actualSteal = Mathf.Min(currentStealAmount, PaintStateManager.Instance.CountdownPaintState.TimeRemaining);
        PaintStateManager.Instance.RemoveTimeFromCountdown(actualSteal);

        float healAmount = actualSteal * healPerTimeStolen;
        damageable.Heal(healAmount);

        OnStealTime?.Invoke();
        Debug.Log($"Boss stole {actualSteal} seconds and healed {healAmount} HP!");
    }

    private void HandleTakeDamage(float currentHealth)
    {
        float damageAmount = lastHealth - currentHealth;
        lastHealth = currentHealth;

        if (damageAmount <= 0f)
            return;

        CheckPhaseTransition(currentHealth);

        damageHistory.Enqueue((Time.time, damageAmount));
        CleanupOldDamages();

        float totalDamage = CalculateTotalDamage();
        if (totalDamage >= damageThreshold)
        {
            DropTimeShard();
            damageHistory.Clear();
        }
    }

    private void CleanupOldDamages()
    {
        while (damageHistory.Count > 0 && Time.time - damageHistory.Peek().time > damageWindow)
        {
            damageHistory.Dequeue();
        }
    }

    private float CalculateTotalDamage()
    {
        float sum = 0f;
        foreach (var entry in damageHistory)
        {
            sum += entry.dmg;
        }
        return sum;
    }

    private void DropTimeShard()
    {
        Vector3 dropOffset = UnityEngine.Random.insideUnitCircle * dropRadius;
        Vector3 dropPos = transform.position + dropOffset;
        dropPos.z = 0f;

        GameObject shardGO = timeShardPool.GetPrefabFromPool(dropPos);

        shardGO.GetComponent<PooledObject>().SetPool(timeShardPool);

        TimeShard shard = shardGO.GetComponent<TimeShard>();
        shard.SetTimeAmount(timeRestorePerShard);

        Debug.Log("Dropped a time shard!");
    }

    private void CheckPhaseTransition(float currentHealth)
    {
        if (isPhase2)
            return;

        float healthPercentage = currentHealth / damageable.MaxHealth;
        if (healthPercentage <= phase2HealthThreshold)
        {
            EnterPhase2();
        }
    }

    private void EnterPhase2()
    {
        if (isPhase2)
            return;

        isPhase2 = true;
        currentStealAmount *= phase2StealMultiplier;

       Debug.Log("Boss entering Phase 2 - increased steal amount!");
    }

    private void OnDestroy()
    {
        stealTimer?.Cancel();
    }
}

