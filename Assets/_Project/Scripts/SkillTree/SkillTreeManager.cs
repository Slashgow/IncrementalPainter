using System.Collections.Generic;
using UnityEngine;

public class SkillTreeManager : MonoBehaviour
{
    [SerializeField] private inkolorgames.Logger logger;

    [Header("Player Resources")]
    public int availableSkillPoints = 10;
    public long playerCurrency = 1000;

    [Header("Skill Nodes")]
    public List<SkillNode> allSkillNodes;

    private HashSet<string> unlockedSkills = new HashSet<string>();
    private Dictionary<string, float> activeEffects = new Dictionary<string, float>();

    void Start()
    {
        RefreshAllNodes();
    }

    public SkillNode FindNodeBySkillData(SkillData skillData)
    {
        foreach (var node in allSkillNodes)
        {
            if (node != null && node.SkillData == skillData)
                return node;
        }
        return null;
    }


    public bool CanUnlockSkill(SkillData skill)
    {
        if (unlockedSkills.Contains(skill.SkillID))
            return false;

        if (availableSkillPoints < skill.SkillPointCost)
            return false;

        if (playerCurrency < skill.CurrencyCost)
            return false;

        if (skill.RequiredSkills != null)
        {
            foreach (var requiredSkill in skill.RequiredSkills)
            {
                if (!unlockedSkills.Contains(requiredSkill.SkillID))
                    return false;
            }
        }

        return true;
    }

    public void TryUnlockSkill(SkillData skill)
    {
        if (!CanUnlockSkill(skill))
        {
            logger.Log($"Cannot unlock {skill.SkillName}: Requirements not met", this);
            return;
        }

        availableSkillPoints -= skill.SkillPointCost;
        playerCurrency -= skill.CurrencyCost;

        unlockedSkills.Add(skill.SkillID);
        ApplySkillEffect(skill);

        RefreshAllNodes();

        logger.Log($"Unlocked skill: {skill.SkillName}", this);
    }

    void ApplySkillEffect(SkillData skill)
    {
        string key = $"{skill.EffectTarget}_{skill.EffectType}";

        if (!activeEffects.ContainsKey(key))
            activeEffects[key] = 0f;

        switch (skill.EffectType)
        {
            case SkillEffectType.MultiplyValue:
            case SkillEffectType.AddFlat:
                activeEffects[key] += skill.EffectValue;
                break;
        }
    }

    public float GetEffectValue(string effectTarget, SkillEffectType effectType)
    {
        string key = $"{effectTarget}_{effectType}";
        return activeEffects.ContainsKey(key) ? activeEffects[key] : 0f;
    }

    public bool IsSkillUnlocked(string skillID) => unlockedSkills.Contains(skillID);

    void RefreshAllNodes()
    {
        foreach (var node in allSkillNodes)
        {
            if (node != null)
                node.UpdateVisuals();
        }
    }

   // // ============================================
   // // SAVE/LOAD SYSTEM
   // // ============================================
   // void SaveProgress()
   // {
   //     PlayerPrefs.SetInt("SkillPoints", availableSkillPoints);
   //     PlayerPrefs.SetString("PlayerCurrency", playerCurrency.ToString());
   //
   //     string unlockedSkillsData = string.Join(",", unlockedSkills);
   //     PlayerPrefs.SetString("UnlockedSkills", unlockedSkillsData);
   //
   //     PlayerPrefs.Save();
   // }
   //
   // void LoadProgress()
   // {
   //     availableSkillPoints = PlayerPrefs.GetInt("SkillPoints", 10);
   //
   //     string currencyStr = PlayerPrefs.GetString("PlayerCurrency", "1000");
   //     playerCurrency = long.Parse(currencyStr);
   //
   //     string unlockedSkillsData = PlayerPrefs.GetString("UnlockedSkills", "");
   //     if (!string.IsNullOrEmpty(unlockedSkillsData))
   //     {
   //         unlockedSkills = new HashSet<string>(unlockedSkillsData.Split(','));
   //
   //         // Reapply all skill effects
   //         foreach (var node in allSkillNodes)
   //         {
   //             if (unlockedSkills.Contains(node.skillData.skillID))
   //             {
   //                 ApplySkillEffect(node.skillData);
   //             }
   //         }
   //     }
   // }

    public void ResetSkillTree()
    {
        unlockedSkills.Clear();
        activeEffects.Clear();
        availableSkillPoints = 10;
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