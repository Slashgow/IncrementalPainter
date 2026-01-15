using System;
using UnityEngine;

[Serializable]
public class SkillDataPerLevelOfType<T> : ISkillLevelData where T : IFunction
{
    [SerializeField] private SkillData<T> skillData;
    public SkillData<T> SkillData => skillData;

    public bool IsUnlocked => CurrentLevel > skillData.StartingLevel;
    public int CurrentLevel { get; private set; }
    public event Action OnLevelUp;
    public event Action OnLevelDown;
    public string SkillID => skillData.SkillID;
    public float GetCurrentLevelData() => skillData.EffectValue.Evaluate(CurrentLevel);
    public void Initialize()
    {
        CurrentLevel = skillData.StartingLevel;
    }

    public bool CanLevelUp()
    {
        return CurrentLevel < skillData.MaxLevel;
    }

    public void LevelUp()
    {
        if (CurrentLevel < skillData.MaxLevel)
        {
            CurrentLevel++;
            OnLevelUp?.Invoke();
        }
    }

    public void LevelDown()
    {
        if(CurrentLevel > skillData.StartingLevel)
        {
            CurrentLevel--;
            OnLevelDown?.Invoke();
        }
    }
}