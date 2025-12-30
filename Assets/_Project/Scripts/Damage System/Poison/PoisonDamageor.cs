using System;
using inkolorgames;
using UnityEngine;
using UnityEngine.Events;

public class PoisonDamageor : BaseDamageor
{
    [SerializeField] private inkolorgames.Logger logger;

    [Header("Poison DoT Settings")]
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> dotDurationSkillDataPerLevel;
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> dotTickIntervalSkillDataPerLevel;

    public SkillDataPerLevelOfType<FunctionAffine> DotDurationSkillDataPerLevel => dotDurationSkillDataPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> DotTickIntervalSkillDataPerLevel => dotTickIntervalSkillDataPerLevel;

    public float DotDuration => dotDurationSkillDataPerLevel.GetCurrentLevelData();
    public float DotTickInterval => dotTickIntervalSkillDataPerLevel.GetCurrentLevelData();

    [Header("Poison Zone")]
    [SerializeField] private PoolingSystem poisonZonePool;

    public static event Action<float> OnPoisonDamage;
    public UnityEvent<Vector3, Color> OnPaintPoisonDie;

    private void OnEnable() => PaintBlob.OnAnyPaintPoisonDie += PaintBlob_OnAnyPaintPoisonDie;
    private void OnDisable() => PaintBlob.OnAnyPaintPoisonDie -= PaintBlob_OnAnyPaintPoisonDie;

    private void PaintBlob_OnAnyPaintPoisonDie(Vector3 deathWorldPosition, Color color)
    {
        OnPaintPoisonDie?.Invoke(deathWorldPosition, color);
        SpawnPoisonZone(deathWorldPosition, color);
    }

    private void SpawnPoisonZone(Vector3 position, Color color)
    {
        if (poisonZonePool == null)
        {
            logger.LogError("Poison Zone pool is not assigned!", this);
            return;
        }

        GameObject zoneInstance = poisonZonePool.GetPrefabFromPool(position);

        if(zoneInstance.TryGetComponent<PooledObject>(out var pooledObject))
        {
            pooledObject.SetPool(poisonZonePool);
            pooledObject.SetDuration(DotDuration + 1f); 
        }
         
        if (zoneInstance.TryGetComponent<PoisonZone>(out var poisonZone))
        {
            poisonZone.Initialize(this,  
                new PoisonZoneData(DamageRadius,Damage,DotDuration,DotTickInterval,CriticalHitLuck,CriticalDamageMultiplier,color,damageableLayers));
        }
        else
        {
            logger.LogError("PoisonZone component not found on prefab!", this);
            Destroy(zoneInstance);
        }
    }

    public override void NotityDamage(float damage) => OnPoisonDamage?.Invoke(damage);
}
