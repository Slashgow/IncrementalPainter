using System.Collections.Generic;
using UnityEngine;

public class SkillTreeManager : MonoBehaviour, ISavable, ILoadable<SkillTreeSaveData> 
{
    [SerializeField] private inkolorgames.Logger logger;
    [SerializeField] private Camera mainCamera;
    public Vector3 GetScreenPosition(Vector3 worldPosition) => mainCamera.WorldToScreenPoint(worldPosition);

    [SerializeField] private AutoClickerDamageor autoClickerDamageor;
    [SerializeField] private PaintStateManager paintStateManager;
    [SerializeField] private AutoClicker autoClicker;
    [SerializeField] private PaintSpawner paintSpawner;
    [SerializeField] private DeathPainter deathPainter;
    [SerializeField] private BombDamageor bombDamageor;
    [SerializeField] private BombStunner bombStunner;
    [SerializeField] private BrushSwipeDamageor brushSwipeDamageor;

    [Header("Skill Nodes")]
    public List<SkillNode> allSkillNodes;

    [Header("Detail UI")]
    [SerializeField] private SkillNodeDetail detailPrefab;
    public SkillNodeDetail DetailPrefab => detailPrefab;
    [SerializeField] private Transform detailCanvas;
    public Transform DetailCanvas => detailCanvas;

    [SerializeField] private Vector2 detailOffset = new Vector2(0f, -10f);
    public Vector2 DetailOffset => detailOffset;

    [Header("Visual")]
    [SerializeField] private Color lockedColor = Color.gray;
    [SerializeField] private Color lockedColorDarker = Color.gray;
    public Color LockedColor => lockedColor;
    public Color LockedColorDarker => lockedColorDarker;

    [SerializeField] private Color unlockedColor = Color.green;
    [SerializeField] private Color unlockedColorDarker = Color.green;
    public Color UnlockedColor => unlockedColor;
    public Color UnlockedColorDarker => unlockedColorDarker;

    [SerializeField] private Color availableColor = Color.yellow;
    [SerializeField] private Color availableColorDarker = Color.yellow;
    public Color AvailableColor => availableColor;
    public Color AvailableColorDarker => availableColorDarker;

    private Dictionary<string, ISkillLevelData> leveledSkills = new();

    private void Awake()
    {
        InitializeSkills();
        SkillTreeSaveData  skillTreeSaveData = Load();
        ApplyLoadedData(skillTreeSaveData);
        GameManager.OnStartGameState += GameManager_OnStartGameState;
    }

    private void OnDestroy()
    {
        if(GameManager.HasInstance)
            GameManager.OnStartGameState -= GameManager_OnStartGameState;
    }
    private void GameManager_OnStartGameState(GameManager.GameState gameState)
    {
        if(gameState != GameManager.GameState.UPGRADE)
            return;

        RefreshAllNodes();
    }

    private void InitializeSkills()
    {
        leveledSkills[autoClickerDamageor.DamageSkillDataPerLevel.SkillID] = autoClickerDamageor.DamageSkillDataPerLevel;
        leveledSkills[autoClickerDamageor.CriticalDamageMultiplierSkillDataPerLevel.SkillID] = autoClickerDamageor.CriticalDamageMultiplierSkillDataPerLevel;
        leveledSkills[autoClickerDamageor.CriticalHitLuckSkillDataPerLevel.SkillID] = autoClickerDamageor.CriticalHitLuckSkillDataPerLevel;
        leveledSkills[autoClickerDamageor.DamageRadiusSkillDataPerLevel.SkillID] = autoClickerDamageor.DamageRadiusSkillDataPerLevel;
        leveledSkills[paintStateManager.TimeOfPaintStatePerLevel.SkillID] = paintStateManager.TimeOfPaintStatePerLevel;
        leveledSkills[paintStateManager.ChanceOfIncreasingTimerPerLevel.SkillID] = paintStateManager.ChanceOfIncreasingTimerPerLevel;
        leveledSkills[paintStateManager.TimeToAddOnIncreasePerLevel.SkillID] = paintStateManager.TimeToAddOnIncreasePerLevel;
        leveledSkills[autoClicker.ClickTimerIntervalPerLevel.SkillID] = autoClicker.ClickTimerIntervalPerLevel;
        leveledSkills[paintSpawner.ChanceOfSpawningWhenKillingPerLevel.SkillID] = paintSpawner.ChanceOfSpawningWhenKillingPerLevel;
        leveledSkills[paintSpawner.ChanceOfSpawningBombPaintPerLevel.SkillID] = paintSpawner.ChanceOfSpawningBombPaintPerLevel;
        leveledSkills[paintSpawner.ChanceOfSpawningFreezePaintPerLevel.SkillID] = paintSpawner.ChanceOfSpawningFreezePaintPerLevel;
        leveledSkills[paintSpawner.ChanceOfSpawningBrushSwipePaintPerLevel.SkillID] = paintSpawner.ChanceOfSpawningBrushSwipePaintPerLevel;
        leveledSkills[deathPainter.SplatterScalePerLevel.SkillID] = deathPainter.SplatterScalePerLevel;
        leveledSkills[bombDamageor.DamageSkillDataPerLevel.SkillID] = bombDamageor.DamageSkillDataPerLevel;
        leveledSkills[bombDamageor.CriticalDamageMultiplierSkillDataPerLevel.SkillID] = bombDamageor.CriticalDamageMultiplierSkillDataPerLevel;
        leveledSkills[bombDamageor.CriticalHitLuckSkillDataPerLevel.SkillID] = bombDamageor.CriticalHitLuckSkillDataPerLevel;
        leveledSkills[bombDamageor.DamageRadiusSkillDataPerLevel.SkillID] = bombDamageor.DamageRadiusSkillDataPerLevel;
        leveledSkills[bombStunner.StunDurationSkillDataPerLevel.SkillID] = bombStunner.StunDurationSkillDataPerLevel;
        leveledSkills[bombStunner.StunRadiusSkillDataPerLevel.SkillID] = bombStunner.StunRadiusSkillDataPerLevel;
        leveledSkills[brushSwipeDamageor.DamageSkillDataPerLevel.SkillID] = brushSwipeDamageor.DamageSkillDataPerLevel;
        leveledSkills[brushSwipeDamageor.CriticalDamageMultiplierSkillDataPerLevel.SkillID] = brushSwipeDamageor.CriticalDamageMultiplierSkillDataPerLevel;
        leveledSkills[brushSwipeDamageor.CriticalHitLuckSkillDataPerLevel.SkillID] = brushSwipeDamageor.CriticalHitLuckSkillDataPerLevel;
        leveledSkills[brushSwipeDamageor.DamageWidthPerLevel.SkillID] = brushSwipeDamageor.DamageWidthPerLevel;
        leveledSkills[brushSwipeDamageor.SwipeLengthPerLevel.SkillID] = brushSwipeDamageor.SwipeLengthPerLevel;
    
        autoClickerDamageor.DamageSkillDataPerLevel.Initialize();
        autoClickerDamageor.CriticalDamageMultiplierSkillDataPerLevel.Initialize();
        autoClickerDamageor.CriticalHitLuckSkillDataPerLevel.Initialize();
        autoClickerDamageor.DamageRadiusSkillDataPerLevel.Initialize();
        paintStateManager.TimeOfPaintStatePerLevel.Initialize();
        paintStateManager.ChanceOfIncreasingTimerPerLevel.Initialize();
        paintStateManager.TimeToAddOnIncreasePerLevel.Initialize();
        autoClicker.ClickTimerIntervalPerLevel.Initialize();
        paintSpawner.ChanceOfSpawningWhenKillingPerLevel.Initialize();
        paintSpawner.ChanceOfSpawningBombPaintPerLevel.Initialize();
        paintSpawner.ChanceOfSpawningFreezePaintPerLevel.Initialize();
        paintSpawner.ChanceOfSpawningBrushSwipePaintPerLevel.Initialize();
        deathPainter.SplatterScalePerLevel.Initialize();
        bombDamageor.DamageSkillDataPerLevel.Initialize();
        bombDamageor.CriticalDamageMultiplierSkillDataPerLevel.Initialize();
        bombDamageor.CriticalHitLuckSkillDataPerLevel.Initialize();
        bombDamageor.DamageRadiusSkillDataPerLevel.Initialize();
        bombStunner.StunDurationSkillDataPerLevel.Initialize();
        bombStunner.StunRadiusSkillDataPerLevel.Initialize();
        brushSwipeDamageor.DamageSkillDataPerLevel.Initialize();
        brushSwipeDamageor.CriticalHitLuckSkillDataPerLevel.Initialize();
        brushSwipeDamageor.CriticalDamageMultiplierSkillDataPerLevel.Initialize();
        brushSwipeDamageor.DamageWidthPerLevel.Initialize();
        brushSwipeDamageor.SwipeLengthPerLevel.Initialize();
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
       
        if (levelRequirement.RequiredSkills != null)
        {
            foreach (var requiredSkillWithLevel in levelRequirement.RequiredSkills)
            {
                if (requiredSkillWithLevel.SkillData == null)
                    continue;

                int currentLevel = GetSkillLevel(requiredSkillWithLevel.SkillData.SkillID);
                if (currentLevel < requiredSkillWithLevel.RequiredLevel)
                {
                    logger.Log($"Cannot level up {skillData.SkillName}: Requires {requiredSkillWithLevel.SkillData.SkillName} Level {requiredSkillWithLevel.RequiredLevel} (currently {currentLevel})", this);
                    return false;
                }
            }
        }

        int cost = skillData.GetCostForLevel(targetLevel);
        if (!CurrencyManager.Instance.CanAfford(cost))
            return false;
        
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

        int cost = skillData.GetCostForLevel(targetLevel);
        CurrencyManager.Instance.AddCurrency(-cost);

        skillLevelData.LevelUp();

        RefreshAllNodes();
        logger.Log($"Leveled up {skillData.SkillName} → Level {skillLevelData.CurrentLevel} (Value: {skillLevelData.GetCurrentLevelData():F2}) [Cost: {cost}]", this);
        Save();
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
        RefreshAllNodes();
    }

    public void Save()
    {
        var saveData = new SkillTreeSaveData(leveledSkills);
        GameSaveManager.Instance.SaveSkillTree(saveData);
    }

    public SkillTreeSaveData Load()
    {
        return GameSaveManager.Instance.LoadSkillTree();
    }

    private void ApplyLoadedData(SkillTreeSaveData saveData)
    {
        if (saveData == null || saveData.skillLevels == null)
            return;

        foreach (var kvp in saveData.skillLevels)
        {
            if (leveledSkills.TryGetValue(kvp.Key, out var skillData))
            {
                for (int i = skillData.CurrentLevel; i < kvp.Value; i++)
                {
                    if (skillData.CanLevelUp())
                        skillData.LevelUp();
                }
            }
        }

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