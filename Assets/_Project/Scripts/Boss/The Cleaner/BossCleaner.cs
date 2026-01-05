using System;
using System.Collections.Generic;
using inkolorgames;
using UnityEngine;
using UnityTimer;

public class BossCleaner : MonoBehaviour
{
    [Header("Boss Stats")]
    [SerializeField] private SimpleDamageable damageable;

    [Header("Water Attack Settings")]
    [SerializeField] private PoolingSystem waterProjectilePool;
    [SerializeField, Range(0f,20f)] private float waterAttackInterval = 5f;
    [SerializeField, Range(0, 6)] private int waterProjectilesPerAttack = 3;
    [SerializeField] private PoolingSystem waterTargetPreviewPool;

    [Header("Healing Settings")]
    [SerializeField] private PoolingSystem healingPaintPool;
    [SerializeField, Range(0,20)] private int healingPaintCountPerWaterHit = 5;
    [SerializeField] private Vector2 spawnOffsetRange;
    [SerializeField, Range(0f,20f)] private float healAmountPerPaint = 5f;

    private Timer attackTimer;
    private List<GameObject> activeWaterProjectiles = new List<GameObject>();

    private SpriteRenderer frameRenderer;

    private void Start()
    {
        if (frameRenderer == null)
            frameRenderer = LevelManager.Instance.CurrentLevelInstance.FrameRenderer;

        StartWaterAttacks();
    }

    private void StartWaterAttacks()
    {
        attackTimer = Timer.Register(waterAttackInterval, onComplete: () => PerformWaterAttack(), isLooped: true,useRealTime: false);
    }

    public void StopWaterAttacks() => attackTimer?.Cancel();

    private void PerformWaterAttack()
    {
        for (int i = 0; i < waterProjectilesPerAttack; i++)
        {
            Vector3 targetPosition = SpriteUtility.GetRandomPositionInSprite(frameRenderer.transform, frameRenderer, true);
            SpawnWaterTargetPreview(targetPosition);
            SpawnWaterProjectile(targetPosition);
        }
    }

    private void SpawnWaterTargetPreview(Vector3 targetPosition)
    {
        GameObject waterPreviewInstance = waterTargetPreviewPool.GetPrefabFromPool(targetPosition);

        if(waterTargetPreviewPool.TryGetComponent<PooledObject>(out var pooledObject))
        {
            pooledObject.SetPool(waterTargetPreviewPool);
        }
    }

    private void SpawnWaterProjectile(Vector3 targetPosition)
    {
        GameObject waterInstance = waterProjectilePool.GetPrefabFromPool(transform.position);

        if (waterInstance.TryGetComponent<WaterProjectile>(out var waterProjectile))
        {
            waterProjectile.Initialize(waterProjectilePool,this, targetPosition);
            activeWaterProjectiles.Add(waterInstance);
        }
    }

    public void SpawnHealingPaint(Vector3 paintPosition)
    {
        for (int i = 0; i < healingPaintCountPerWaterHit; i++)
        {
            GameObject healingInstance = healingPaintPool.GetPrefabFromPool(paintPosition);

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
        attackTimer?.Cancel();

        foreach (var projectile in activeWaterProjectiles)
        {
            if (projectile != null)
                Destroy(projectile);
        }
        activeWaterProjectiles.Clear();
    }
}