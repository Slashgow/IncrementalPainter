using System.Collections.Generic;
using UnityEngine;

public class SkillTreeManager : MonoBehaviour 
{
    [SerializeField] private inkolorgames.Logger logger;
    [SerializeField] private SimpleDamageor autoClickerDamageor;

    [Header("Player Resources")]
    public int availableSkillPoints = 10;
    public long playerCurrency = 1000;

    [Header("Skill Nodes")]
    public List<SkillNode> allSkillNodes;

    private Dictionary<string, ISkillLevelData> leveledSkills = new();

    private void Awake()
    {
        InitializeSkills();
    }

    private void InitializeSkills()
    {
        leveledSkills[autoClickerDamageor.DamageSkillDataPerLevel.SkillID] = autoClickerDamageor.DamageSkillDataPerLevel;
        leveledSkills[autoClickerDamageor.CriticalDamageSkillDataPerLevel.SkillID] = autoClickerDamageor.CriticalDamageSkillDataPerLevel;
        autoClickerDamageor.DamageSkillDataPerLevel.Initialize();
        autoClickerDamageor.CriticalDamageSkillDataPerLevel.Initialize();
    }

    void Start()
    {
        RefreshAllNodes();
    }

    public SkillNode FindNodeBySkillData(SkillDataBase skillData)
    {
        foreach (var node in allSkillNodes)
        {
            if (node != null && node.SkillDataBase == skillData)
                return node;
        }
        return null;
    }
    public SkillNode FindNodeBySkillDataAndLevel(SkillDataBase skillData, int level)
    {
        foreach (var node in allSkillNodes)
        {
            if (node != null && node.SkillDataBase == skillData && node.TargetLevel == level)
                return node;
        }
        return null;
    }

    public int GetSkillLevel(string skillID)
    {
        if (leveledSkills.TryGetValue(skillID, out var skillLevelData))
            return skillLevelData.CurrentLevel;
        return 0;
    }

    public bool CanLevelUpToLevel(SkillDataBase skillData, int targetLevel)
    {
        if (!leveledSkills.TryGetValue(skillData.SkillID, out var skillLevelData))
        {
            logger.Log($"Skill not initialized: {skillData.SkillName}", this);
            return false;
        }

        if (skillLevelData.CurrentLevel >= targetLevel)
            return false;

        if (skillLevelData.CurrentLevel < targetLevel - 1)
            return false;

        if (!skillLevelData.CanLevelUp())
            return false;

        var levelRequirement = skillData.GetRequirementsForLevel(targetLevel);
        if (levelRequirement == null)
        {
            if (availableSkillPoints < 1)
                return false;
        }
        else
        {
            if (levelRequirement.RequiredSkills != null)
            {
                foreach (var requiredSkill in levelRequirement.RequiredSkills)
                {
                    if (!IsSkillUnlocked(requiredSkill.SkillID))
                        return false;
                }
            }

            if (!CurrencyManager.Instance.CanAfford(levelRequirement.CurrencyCost))
                return false;
        }

        return true;
    }

    public void TryLevelUpSkillToLevel(SkillDataBase skillData, int targetLevel)
    {
        if (!leveledSkills.TryGetValue(skillData.SkillID, out var skillLevelData))
            return;

        if (!CanLevelUpToLevel(skillData, targetLevel))
        {
            logger.Log($"Cannot level up {skillData.SkillName} to level {targetLevel}: Requirements not met", this);
            return;
        }

        var levelRequirement = skillData.GetRequirementsForLevel(targetLevel);
        if (levelRequirement != null)
            CurrencyManager.Instance.AddCurrency(-levelRequirement.CurrencyCost);

        skillLevelData.LevelUp();

        RefreshAllNodes();

        logger.Log($"Leveled up {skillData.SkillName} → Level {skillLevelData.CurrentLevel} (Value: {skillLevelData.GetCurrentLevelData():F2})", this);
    }


    public bool IsSkillUnlocked(string skillID) => leveledSkills[skillID].IsUnlocked;

    void RefreshAllNodes()
    {
        foreach (var node in allSkillNodes)
        {
            if (node != null)
                node.UpdateVisuals();
        }
    }

    public void ResetSkillTree()
    {
        foreach (var skill in leveledSkills.Values)
        {
            skill.Initialize();
        }
 
        availableSkillPoints = 10;
        playerCurrency = 1000000;
        RefreshAllNodes();
    }

#if UNITY_EDITOR
    [ContextMenu("Create All Connection Lines")]
    public void CreateAllConnectionLines()
    {
        int linesCreated = 0;
        foreach (var node in allSkillNodes)
        {
            if (node != null)
            {
                node.CreateConnectionsToRequiredSkills();
                linesCreated += node.ConnectionLines.Count;
            }
        }
        Debug.Log($"Created {linesCreated} connection lines across {allSkillNodes.Count} nodes");
    }

    [ContextMenu("Clear All Connection Lines")]
    public void ClearAllConnectionLines()
    {
        foreach (var node in allSkillNodes)
        {
            if (node != null)
            {
                node.ClearConnectionLines();
            }
        }
        Debug.Log("Cleared all connection lines");
    }
#endif
}