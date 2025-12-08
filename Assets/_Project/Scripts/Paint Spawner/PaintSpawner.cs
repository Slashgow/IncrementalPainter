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
    [SerializeField] private bool autoStart = true;
    [SerializeField] private float zOffset = 0f;
    [SerializeField] private Transform spawnParent;
    [SerializeField] private bool useRealTime = false;
    
    private SpriteRenderer spriteRenderer;

    private Timer spawnTimer;
    private int spawnedCount = 0;

    protected override void Awake()
    {
        base.Awake();
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
        for (int i = 0; i < InitialCount; i++)
        {
            SpawnObject();
        }

        spawnTimer?.Cancel();
        spawnTimer = Timer.Register(SpawnTimeInterval, onComplete:SpawnObject, isLooped: true, useRealTime: useRealTime);
    }

    public void StopSpawning() => spawnTimer?.Cancel();

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
        GameObject spawned = pool.GetPrefabFromPool(position, spawnParent, false);

        if (spawned.TryGetComponent<SimpleDamageable>(out var damageable))
            damageable.InitializePool(pool);

        if (spawned.TryGetComponent<PaintBlob>(out var paintBlob))
            paintBlob.Initialize(paintType);

        if (spawned.TryGetComponent<SimpleSplittable>(out var splittable))
        {
            splittable.SetGeneration(splitGeneration);
            float scale = customScale ?? transform.localScale.x; 
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

    private void SimpleDamageable_OnAnyDamageableDie(Vector3 arg1, Color arg2)
    {
        spawnedCount--;
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