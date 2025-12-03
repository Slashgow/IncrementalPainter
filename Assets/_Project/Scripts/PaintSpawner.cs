using System;
using inkolorgames;
using UnityEngine;
using UnityTimer;

public class PaintSpawner : MonoBehaviour
{
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> chanceOfSpawningWhenKillingPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> ChanceOfSpawningWhenKillingPerLevel => chanceOfSpawningWhenKillingPerLevel;
    public float ChanceOfSpawnOnKill => chanceOfSpawningWhenKillingPerLevel.GetCurrentLevelData();

    [Header("Spawn Settings")]
    [SerializeField] private PoolingSystem pool;
    [SerializeField] private float spawnInterval = 1f;
    [SerializeField] private bool autoStart = true;
    [SerializeField] private int maxSpawnCount = -1;
    [SerializeField] private float zOffset = 0f;
    [SerializeField] private Transform spawnParent;
    [SerializeField] private bool useRealTime = false;
    
    private SpriteRenderer spriteRenderer;

    private Timer spawnTimer;
    private int spawnedCount = 0;

    private void Awake()
    {
        SimpleDamageable.OnAnyDamageableDie += SimpleDamageable_OnAnyDamageableDie;
    }

    void Start()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = LevelManager.Instance.CurrentLevelInstance.FrameRenderer;
        }

        if (autoStart)
            StartSpawning();
    }

    private void OnDestroy()
    {
        SimpleDamageable.OnAnyDamageableDie -= SimpleDamageable_OnAnyDamageableDie;
        StopSpawning();
    }

    public void StartSpawning()
    {
        spawnTimer?.Cancel();
        spawnTimer = Timer.Register(spawnInterval, onComplete:SpawnObject, isLooped: true, useRealTime: useRealTime);
    }

    public void StopSpawning() => spawnTimer?.Cancel();

    public void SpawnObject()
    {
        if (maxSpawnCount >= 0 && spawnedCount >= maxSpawnCount)
        {
            StopSpawning();
            return;
        }

        Vector3 spawnPosition = SpriteUtility.GetRandomPositionInSprite(spriteRenderer.transform, spriteRenderer);
        spawnPosition.z += zOffset;
        GameObject spawned = pool.GetPrefabFromPool(spawnPosition, spawnParent, false);
        spawnedCount++;
    }

    private void SimpleDamageable_OnAnyDamageableDie(Vector3 arg1, Color arg2) => TrySpawn();
    private void TrySpawn()
    {
        if(LuckUtility.RollLuck(ChanceOfSpawnOnKill))
            SpawnObject();
    }

    void OnDrawGizmosSelected()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null && spriteRenderer.sprite != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(spriteRenderer.bounds.center, spriteRenderer.bounds.size);
        }
    }
}