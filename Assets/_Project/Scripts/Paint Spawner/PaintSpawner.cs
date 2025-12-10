using System;
using System.Collections.Generic;
using inkolorgames;
using UnityEngine;
using UnityTimer;

public class PaintSpawner : MonoSingleton<PaintSpawner>
{
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> initialCountPerLevel;
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> spawnTimeIntervalPerLevel;
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> maxSpawnCountPerLevel;
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> chanceOfSpawningWhenKillingPerLevel;
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> chanceOfSpawningBombPaintPerLevel;
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> chanceOfSpawningFreezePaintPerLevel;
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> chanceOfSpawningBrushSwipePaintPerLevel;
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> chanceOfSpawningSplitPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> InitialCountPerLevel => initialCountPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> SpawnTimeIntervalPerLevel => spawnTimeIntervalPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> MaxSpawnCountPerLevel => maxSpawnCountPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> ChanceOfSpawningWhenKillingPerLevel => chanceOfSpawningWhenKillingPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> ChanceOfSpawningBombPaintPerLevel => chanceOfSpawningBombPaintPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> ChanceOfSpawningFreezePaintPerLevel => chanceOfSpawningFreezePaintPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> ChanceOfSpawningBrushSwipePaintPerLevel => chanceOfSpawningBrushSwipePaintPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> ChanceOfSpawningSplitPerLevel => chanceOfSpawningSplitPerLevel;
    public int InitialCount => Mathf.FloorToInt(initialCountPerLevel.GetCurrentLevelData());
    public float SpawnTimeInterval => spawnTimeIntervalPerLevel.GetCurrentLevelData();
    public int MaxSpawnCount => Mathf.FloorToInt(maxSpawnCountPerLevel.GetCurrentLevelData());
    public float ChanceOfSpawnOnKill => chanceOfSpawningWhenKillingPerLevel.GetCurrentLevelData();
    public float ChanceOfSpawningBombPaint => chanceOfSpawningBombPaintPerLevel.GetCurrentLevelData();
    public float ChanceOfSpawningFreezePaint => chanceOfSpawningFreezePaintPerLevel.GetCurrentLevelData();
    public float ChanceOfSpawningBrushSwipePaint => chanceOfSpawningBrushSwipePaintPerLevel.GetCurrentLevelData();
    public float ChanceOfSpawningSplitPaint => chanceOfSpawningSplitPerLevel.GetCurrentLevelData();

    [Header("Spawn Settings")]
    [SerializeField] private PoolingSystem pool;
    [SerializeField] private EnemyData[] enemyDatas;
    [SerializeField] private bool autoStart = true;
    [SerializeField] private float zOffset = 0f;
    [SerializeField] private Transform spawnParent;
    [SerializeField] private bool useRealTime = false;
    
    private SpriteRenderer spriteRenderer;

    private Timer spawnTimer;
    private int spawnedCount = 0;
    private bool isSpawning;
    private bool isInitialized;

    protected override void Awake()
    {
        base.Awake();
        isInitialized = false;
        SimpleDamageable.OnAnyDamageableDie += SimpleDamageable_OnAnyDamageableDie;
    }

    void Start()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = LevelManager.Instance.CurrentLevelInstance.FrameRenderer;
        }

        PaintStateManager.OnStartPaintState += PaintStateManager_OnStartPaintState;

        if (autoStart && !isInitialized)
            InitializeSpawning();
    }

    private void OnDestroy()
    {
        if(PaintStateManager.HasInstance)
            PaintStateManager.OnStartPaintState -= PaintStateManager_OnStartPaintState;

        SimpleDamageable.OnAnyDamageableDie -= SimpleDamageable_OnAnyDamageableDie;
        StopSpawning();
    }
    private void PaintStateManager_OnStartPaintState() => InitializeSpawning();

    public void InitializeSpawning()
    {
        isInitialized = true;
        for (int i = 0; i < InitialCount; i++)
        {
            SpawnObject();
        }

        StartSpawning();
    }

    private void StartSpawning()
    {
        isSpawning = true;
        spawnTimer?.Cancel();
        spawnTimer = Timer.Register(SpawnTimeInterval, onComplete: SpawnObject, isLooped: true, useRealTime: useRealTime);
    }

    public void StopSpawning()
    {
        isSpawning = false;
        spawnTimer?.Cancel();
    }

    public void SpawnObject()
    {
        if (MaxSpawnCount >= 0 && spawnedCount >= MaxSpawnCount)
        {
            StopSpawning();
            return;
        }

        Vector3 spawnPosition = SpriteUtility.GetRandomPositionInSprite(spriteRenderer.transform, spriteRenderer);
        spawnPosition.z += zOffset;

        PaintType paintType = GetWeightedPaintType();
        SpawnPaintBlob(spawnPosition, paintType);
    }

  

    public GameObject SpawnPaintBlob(Vector3 position, PaintType paintType, int splitGeneration = 0, float? customScale = null)
    {
        EnemyData enemyData = ChooseEnemyData(SkillTreeManager.TotalUpgradesBought, LevelManager.CurrentLevelIndex);
        //Debug.Log($"Choose Ennemy {enemyData.Difficulty.ToString()}");

        GameObject spawned = pool.GetPrefabFromPool(position, spawnParent, false);

        spawned.transform.localScale = Vector3.one * enemyData.Scale;

        if(spawned.TryGetComponent<SimpleCostable>(out var costable))
            costable.Initalize(splitGeneration == 0 ? enemyData.Cost : Mathf.FloorToInt(enemyData.Cost * 0.5f / splitGeneration));

        if (spawned.TryGetComponent<SimpleDamageable>(out var damageable))
            damageable.Initialize(pool, enemyData.MaxHealth);

        if (spawned.TryGetComponent<PaintBlob>(out var paintBlob))
            paintBlob.Initialize(paintType);

        if (spawned.TryGetComponent<SimpleSplittable>(out var splittable))
        {
            splittable.SetGeneration(splitGeneration);
            float scale = customScale ?? spawned.transform.localScale.x; 
            if (customScale == null && splitGeneration > 0)
            {
                float baseScale = 1f;
                scale = SplitUtility.CalculateSplitScale(baseScale, splitGeneration, splittable.ScaleMultiplier);
            }
            spawned.transform.localScale = Vector3.one * scale;
        }

        spawnedCount++;
        return spawned;
    }

    private void SimpleDamageable_OnAnyDamageableDie(Vector3 arg1, Color arg2, float scale)
    {
        spawnedCount--;

        if (!isSpawning && spawnedCount < MaxSpawnCount)
            StartSpawning();

        TrySpawn();
    }

    private void TrySpawn()
    {
        if(LuckUtility.RollLuck(ChanceOfSpawnOnKill))
            SpawnObject();
    }

    private PaintType GetWeightedPaintType()
    {
        float totalSpecialWeight = ChanceOfSpawningBombPaint + ChanceOfSpawningFreezePaint + ChanceOfSpawningBrushSwipePaint + ChanceOfSpawningSplitPaint;

        if (totalSpecialWeight <= 0f)
            return PaintType.Normal;

        if (!LuckUtility.RollLuck(totalSpecialWeight))
            return PaintType.Normal;

        float roll = UnityEngine.Random.Range(0f, totalSpecialWeight);
        float cumulativeWeight = 0f;

        cumulativeWeight += ChanceOfSpawningBombPaint;
        if (roll < cumulativeWeight)
            return PaintType.BombPaint;

        cumulativeWeight += ChanceOfSpawningFreezePaint;
        if (roll < cumulativeWeight)
            return PaintType.Freeze;

        cumulativeWeight += ChanceOfSpawningBrushSwipePaint;
        if (roll < cumulativeWeight)
            return PaintType.BrushSwipe;

        return PaintType.Split;
    }

    public EnemyData ChooseEnemyData(int upgrades, int level)
    {
        float totalWeight = 0f;
        float[] weights = new float[enemyDatas.Length];

        for (int i = 0; i < enemyDatas.Length; i++)
        {
            weights[i] = enemyDatas[i].BaseWeigth *
                         Mathf.Pow(upgrades, enemyDatas[i].UpgradeWeightExponent) *
                         Mathf.Pow(level, enemyDatas[i].LevelWeightExponent);
            totalWeight += weights[i];
        }

        float roll = UnityEngine.Random.value * totalWeight;
        for (int i = 0; i < enemyDatas.Length; i++)
        {
            if (roll < weights[i])
                return enemyDatas[i];

            roll -= weights[i];
        }
        return enemyDatas[0];
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