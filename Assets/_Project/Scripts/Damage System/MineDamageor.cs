using System;
using UnityEngine;
using UnityEngine.Events;

public class MineDamageor : BaseDamageor
{
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> spawnChanceSkillDataPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> SpawnChanceSkillDataPerLevel => spawnChanceSkillDataPerLevel;
    public float SpawnChance => spawnChanceSkillDataPerLevel.GetCurrentLevelData();

    [Header("Mine Spawning")]
    [SerializeField] private Mine minePrefab;
    [SerializeField] private Transform mineParent; // Optional: keeps hierarchy clean

    public static event Action<float> OnMineDamage;
    public UnityEvent<Vector3> OnMineExplode;

    private void OnEnable() => PaintSpawner.OnSpawnPaint += TrySpawnMine;
    private void OnDisable() => PaintSpawner.OnSpawnPaint -= TrySpawnMine;

    private void TrySpawnMine()
    {
        if (LuckUtility.RollLuck(SpawnChance))
        {
            SpawnMine(PaintSpawner.Instance.GetSpawnPosition());
        }
    }

    public void NotifyMineExplosion(Vector3 position) => OnMineExplode?.Invoke(position);
    public override void NotityDamage(float damage) => OnMineDamage?.Invoke(damage);

    private void SpawnMine(Vector3 worldPosition)
    {
        if (minePrefab == null)
        {
            Debug.LogWarning("[MineDamageor] No mine prefab assigned!", this);
            return;
        }

        worldPosition.z = 0f; // Keep 2D-friendly

        Mine mine = Instantiate(minePrefab, worldPosition, Quaternion.identity, mineParent);

        mine.Initialize(this);
    }
}
