using UnityEngine;

public class BombDamageor : BaseDamageor
{
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> luckToProcOnKillPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> LuckToProcOnKillPerLevel => luckToProcOnKillPerLevel;
    public float LuckToProcOnKill => luckToProcOnKillPerLevel.GetCurrentLevelData();

    private void OnEnable() => SimpleDamageable.OnAnyDamageableDie += SimpleDamageable_OnAnyDamageableDie;
    private void OnDisable() => SimpleDamageable.OnAnyDamageableDie -= SimpleDamageable_OnAnyDamageableDie;

    private void SimpleDamageable_OnAnyDamageableDie(Vector3 deathWorldPosition, Color color)
    {
        if (LuckUtility.RollLuck(LuckToProcOnKill))
            TryDamage(deathWorldPosition);
    }
}