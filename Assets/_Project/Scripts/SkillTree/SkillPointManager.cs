using System;
using inkolorgames;
using UnityEngine;

public class SkillPointManager : MonoSingleton<SkillPointManager>
{
    [SerializeField] private inkolorgames.Logger logger;

    private int currentSkillPoints;
    public int CurrentSkillPoints => currentSkillPoints;

    public event Action<int> OnSkillPointsChanged;
    public event Action<int, int> OnSkillPointsGained;
    public event Action<int, int> OnSkillPointsSpent;

    protected override void Awake()
    {
        base.Awake();
        LoadSkillPoints();
    }

    public void AddSkillPoints(int amount)
    {
        if (amount == 0)
            return;

        int oldPoints = currentSkillPoints;
        currentSkillPoints += amount;

        OnSkillPointsChanged?.Invoke(currentSkillPoints);

        if (amount > 0)
        {
            logger.Log($"Gained {amount} skill points. Total: {currentSkillPoints}", this);
            OnSkillPointsGained?.Invoke(amount, currentSkillPoints);
            SaveSkillPoints();
        }
        else
        {
            logger.Log($"Spent {Mathf.Abs(amount)} skill points. Total: {currentSkillPoints}", this);
            OnSkillPointsSpent?.Invoke(Mathf.Abs(amount), currentSkillPoints);
            SaveSkillPoints();
        }
    }

    public bool TrySpendSkillPoints(int amount)
    {
        if (amount < 0)
        {
            logger.LogWarning("TrySpendSkillPoints called with negative amount.", this);
            return false;
        }

        if (currentSkillPoints >= amount)
        {
            AddSkillPoints(-amount);
            return true;
        }

        logger.Log($"Not enough skill points. Need: {amount}, Have: {currentSkillPoints}", this);
        return false;
    }

    public bool CanAfford(int amount) => currentSkillPoints >= amount;

    private void SaveSkillPoints()
    {
        GameSaveManager.Instance.SaveSkillPoints(currentSkillPoints);
    }

    private void LoadSkillPoints()
    {
        currentSkillPoints = GameSaveManager.Instance.LoadSkillPoints();
        logger.Log($"Loaded {currentSkillPoints} skill points", this);
    }

    public void ResetSkillPoints()
    {
        currentSkillPoints = 0;
        SaveSkillPoints();
        OnSkillPointsChanged?.Invoke(currentSkillPoints);
    }
}