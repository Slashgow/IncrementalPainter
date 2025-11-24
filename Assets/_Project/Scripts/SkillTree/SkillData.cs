using UnityEngine;

public abstract class SkillData<T> : SkillDataBase where T : IFunction
{
    [Header("Effects")]
    [SerializeField] private T effectValue;
    public T EffectValue => effectValue;
    public override float GetEffectValueAtLevel(int level)
    {
        return effectValue.Evaluate(level);
    }
}
