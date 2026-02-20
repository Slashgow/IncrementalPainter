using System;
using UnityEngine;

public class TurretSpawner : MonoBehaviour
{
    [Header("Turret Spawner Parameters")]
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> attackSpeedSkillDataPerLevel;       // Fires per second
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> paintBlobsPerFireSkillDataPerLevel; // Blobs fired per shot
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> reloadTimeSkillDataPerLevel;        // Seconds to reload
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> magazineCapacitySkillDataPerLevel;  // Blobs per magazine
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> turretCountSkillDataPerLevel;       // Number of turrets

    [Header("Boost Parameters")]
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> boostChanceSkillDataPerLevel;              // 0-100
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> boostAttackSpeedMultiplierPerLevel;        // e.g. 2 = double speed
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> boostReloadTimeMultiplierPerLevel;         // e.g. 0.5 = half reload time

  
    public SkillDataPerLevelOfType<FunctionAffine> AttackSpeedSkillDataPerLevel => attackSpeedSkillDataPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> PaintBlobsPerFireSkillDataPerLevel => paintBlobsPerFireSkillDataPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> ReloadTimeSkillDataPerLevel => reloadTimeSkillDataPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> MagazineCapacitySkillDataPerLevel => magazineCapacitySkillDataPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> TurretCountSkillDataPerLevel => turretCountSkillDataPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> BoostChanceSkillDataPerLevel => boostChanceSkillDataPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> BoostAttackSpeedMultiplierPerLevel => boostAttackSpeedMultiplierPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> BoostReloadTimeMultiplierPerLevel => boostReloadTimeMultiplierPerLevel;


    public float AttackSpeed => attackSpeedSkillDataPerLevel.GetCurrentLevelData();
    public int PaintBlobsPerFire => Mathf.RoundToInt(paintBlobsPerFireSkillDataPerLevel.GetCurrentLevelData());
    public float ReloadTime => reloadTimeSkillDataPerLevel.GetCurrentLevelData();
    public int MagazineCapacity => Mathf.RoundToInt(magazineCapacitySkillDataPerLevel.GetCurrentLevelData());
    public int TurretCount => Mathf.RoundToInt(turretCountSkillDataPerLevel.GetCurrentLevelData());

    public float BoostChance => boostChanceSkillDataPerLevel.GetCurrentLevelData();
    public float BoostAttackSpeedMultiplier => boostAttackSpeedMultiplierPerLevel.GetCurrentLevelData();
    public float BoostReloadTimeMultiplier => boostReloadTimeMultiplierPerLevel.GetCurrentLevelData();

    public float AttackInterval => AttackSpeed > 0f ? 1f / AttackSpeed : float.MaxValue;
    public float BoostedAttackInterval => AttackSpeed > 0f ? 1f / (AttackSpeed * BoostAttackSpeedMultiplier) : float.MaxValue;
    public float BoostedReloadTime => ReloadTime * BoostReloadTimeMultiplier;

    public event Action OnFire;
    public event Action OnStartReload;
    public event Action OnFinishReload;
    public event Action OnBoostTriggered;


    public bool TryFire(ref int currentMagazine, ref bool isBoosted)
    {
        if (currentMagazine <= 0)
            return false;

        if (!isBoosted && LuckUtility.RollLuck(BoostChance))
        {
            isBoosted = true;
            OnBoostTriggered?.Invoke();
        }

        currentMagazine -= PaintBlobsPerFire;
        if (currentMagazine < 0) 
            currentMagazine = 0;

        OnFire?.Invoke();
        return true;
    }

    public void NotifyStartReload() => OnStartReload?.Invoke();
    public void NotifyFinishReload() => OnFinishReload?.Invoke();
}
