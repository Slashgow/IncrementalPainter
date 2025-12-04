using inkolorgames;
using UnityEngine;
using UnityTimer;

public class PaintSpawner : MonoBehaviour
{
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> chanceOfSpawningWhenKillingPerLevel;
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> chanceOfSpawningBombPaintPerLevel;
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> chanceOfSpawningFreezePaintPerLevel;
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> chanceOfSpawningBrushSwipePaintPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> ChanceOfSpawningWhenKillingPerLevel => chanceOfSpawningWhenKillingPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> ChanceOfSpawningBombPaintPerLevel => chanceOfSpawningBombPaintPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> ChanceOfSpawningFreezePaintPerLevel => chanceOfSpawningFreezePaintPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> ChanceOfSpawningBrushSwipePaintPerLevel => chanceOfSpawningBrushSwipePaintPerLevel;
    public float ChanceOfSpawnOnKill => chanceOfSpawningWhenKillingPerLevel.GetCurrentLevelData();
    public float ChanceOfSpawningBombPaint => chanceOfSpawningBombPaintPerLevel.GetCurrentLevelData();
    public float ChanceOfSpawningFreezePaint => chanceOfSpawningFreezePaintPerLevel.GetCurrentLevelData();
    public float ChanceOfSpawningBrushSwipePaint => chanceOfSpawningBrushSwipePaintPerLevel.GetCurrentLevelData();

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

        PaintType paintType = GetWeightedPaintType();
        GameObject spawned = pool.GetPrefabFromPool(spawnPosition, spawnParent, false);

        PaintBlob paintBlob = spawned.GetComponent<PaintBlob>();
        if (paintBlob != null)
            paintBlob.Initialize(paintType);

        spawnedCount++;
    }

    private void SimpleDamageable_OnAnyDamageableDie(Vector3 arg1, Color arg2) => TrySpawn();
    private void TrySpawn()
    {
        if(LuckUtility.RollLuck(ChanceOfSpawnOnKill))
            SpawnObject();
    }

    private PaintType GetWeightedPaintType()
    {
        float totalSpecialWeight = ChanceOfSpawningBombPaint + ChanceOfSpawningFreezePaint + ChanceOfSpawningBrushSwipePaint;

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

        return PaintType.BrushSwipe;
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