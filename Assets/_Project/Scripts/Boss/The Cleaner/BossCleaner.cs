using System;
using System.Collections.Generic;
using inkolorgames;
using UnityEngine;
using UnityEngine.Events;
using UnityTimer;

public class BossCleaner : BossPhased
{
    [Header("References")]
    [SerializeField] private PoolingSystem waterTargetPreviewPool;
    [SerializeField] private Transform guntipTransform;

    [Header("Water Attack Settings")]
    [Header("Phase 1 Settings")]
    [SerializeField] private PoolingSystem waterProjectilePool;
    [SerializeField, Range(0f,20f)] private float waterAttackInterval = 5f;
    [SerializeField, Range(0, 6)] private int waterProjectilesPerAttack = 3;

    [Header("Phase 2 Settings")]
    [SerializeField] private PoolingSystem waterProjectilePoolPhase2;
    [SerializeField, Range(1, 10)] private int shotsPerBurst = 3;
    [SerializeField, Range(0f, 2f)] private float timeBetweenShots = 0.5f;
    [SerializeField, Range(0f, 10f)] private float reloadTime = 4f;

    [SerializeField] private UnityEvent<Vector3> OnLaunchWaterAttack; 

    [Header("Healing Settings")]
    [SerializeField] private PoolingSystem healingPaintPool;
    [SerializeField, Range(0,20)] private int healingPaintCountPerWaterHit = 5;
    [SerializeField] private Vector2 spawnOffsetRange;
    [SerializeField, Range(0f,20f)] private float healAmountPerPaint = 5f;

    private Timer attackTimer;
    private Timer burstTimer;
    private Timer reloadTimer;
    private int currentBurstCount = 0;
    private List<GameObject> activeWaterProjectiles = new List<GameObject>();

    private SpriteRenderer frameRenderer;
    private bool isCurrentlyFiringPhase2 = false;
    private void Start()
    {
        if (frameRenderer == null)
            frameRenderer = LevelManager.Instance.CurrentLevelInstance.FrameRenderer;

        if (isPhase2)
            return;

        StartWaterAttacks();
    }

    private void StartWaterAttacks()
    {
        attackTimer = Timer.Register(waterAttackInterval, onComplete: () => PerformWaterAttack(waterProjectilesPerAttack, waterProjectilePool), 
            isLooped: true,useRealTime: false);
    }

    private void StartWaterAttackPhase2()
    {
        isCurrentlyFiringPhase2 = true;

        currentBurstCount = 0;
        StopAllAttackTimers();
        FireBurst();
    }

    private void FireBurst()
    {
        PerformWaterAttack(1, waterProjectilePoolPhase2);
        OnLaunchWaterAttack?.Invoke(guntipTransform.position);
        currentBurstCount++;

        if (currentBurstCount < shotsPerBurst)
        {
            burstTimer = Timer.Register(timeBetweenShots, onComplete: FireBurst, useRealTime: false);
        }
        else
        {
            currentBurstCount = 0;
            reloadTimer = Timer.Register(reloadTime, onComplete: FireBurst, useRealTime: false);
        }
    }

    protected override void EnterPhase2()
    {
        base.EnterPhase2();

        if (isCurrentlyFiringPhase2)
            return;

        StopAllAttackTimers();
        StartWaterAttackPhase2();
    }

    private void StopAllAttackTimers()
    {
        attackTimer?.Cancel();
        burstTimer?.Cancel();
        reloadTimer?.Cancel();
      
        // Optional: clear state
        currentBurstCount = 0;
    }

    private void PerformWaterAttack(int numberOfWaterProjectilePerAttack, PoolingSystem pool)
    {
        OnLaunchWaterAttack?.Invoke(guntipTransform.position);
        for (int i = 0; i < numberOfWaterProjectilePerAttack; i++)
        {
            Vector3 targetPosition = SpriteUtility.GetRandomPositionInSprite(frameRenderer.transform, frameRenderer, true);
            SpawnWaterTargetPreview(targetPosition);
            SpawnWaterProjectile(targetPosition, pool);
        }

    }

    private void SpawnWaterTargetPreview(Vector3 targetPosition)
    {
        GameObject waterPreviewInstance = waterTargetPreviewPool.GetPrefabFromPool(targetPosition, null, true);

        if(waterPreviewInstance.TryGetComponent<PooledObject>(out var pooledObject))
        {
            pooledObject.SetPool(waterTargetPreviewPool);
        }
    }

    private void SpawnWaterProjectile(Vector3 targetPosition, PoolingSystem pool)
    {
        GameObject waterInstance = pool.GetPrefabFromPool(guntipTransform.position,null, true);

        if (waterInstance.TryGetComponent<EraserEnemy>(out var waterProjectile))
        {
            waterProjectile.Initialize(pool, targetPosition, this);
            activeWaterProjectiles.Add(waterInstance);
        }
    }

    public void SpawnHealingPaint(Vector3 paintPosition)
    {
        for (int i = 0; i < healingPaintCountPerWaterHit; i++)
        {
            GameObject healingInstance = healingPaintPool.GetPrefabFromPool(paintPosition,null, true);

            if (healingInstance.TryGetComponent<HealingPaint>(out var healingPaint))
            {
                healingPaint.Initialize(healingPaintPool, damageable, 
                    paintPosition + (Vector3)Helper.CalculateOffset(true, Vector2.zero, spawnOffsetRange), 
                    healAmountPerPaint, this.transform);
            }
        }
    }

    private void OnDestroy()
    {
        StopAllAttackTimers();
        foreach (var projectile in activeWaterProjectiles)
        {
            if (projectile != null)
                Destroy(projectile);
        }
        activeWaterProjectiles.Clear();
    }
}
