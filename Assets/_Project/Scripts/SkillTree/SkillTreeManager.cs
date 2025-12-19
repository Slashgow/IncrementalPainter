using System.Collections;
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
    [SerializeField] private Painter deathPainter;
    [SerializeField] private BombDamageor bombDamageor;
    [SerializeField] private BombStunner bombStunner;
    [SerializeField] private BrushSwipeDamageor brushSwipeDamageor;
    [SerializeField] private CurrencyManager currencyManager;
    [SerializeField] private MagneterManager magneterManager;
    [SerializeField] private Splitter splitter;
    [SerializeField] private Vacuumer vacuumer;

    [Header("Skill Nodes")]
    public List<SkillNode> allSkillNodes;

    [Header("Detail UI")]
    [SerializeField] private SkillNodeDetail detailPrefab;
    public SkillNodeDetail DetailPrefab => detailPrefab;
    [SerializeField] private Transform detailCanvas;
    public Transform DetailCanvas => detailCanvas;

    [SerializeField] private Vector2 detailOffset = new Vector2(0f, -10f);
    public Vector2 DetailOffset => detailOffset;

    [SerializeField] private SkillTreeVisualData skillTreeVisualData;
    public SkillTreeVisualData SkillTreeVisualData => skillTreeVisualData;

    private Dictionary<string, ISkillLevelData> leveledSkills = new();

    private static int totalUpgradesBought = 0;
    public static int TotalUpgradesBought => totalUpgradesBought;

    private void Awake()
    {
        InitializeSkills();
        SkillTreeSaveData  skillTreeSaveData = Load();
        ApplyLoadedData(skillTreeSaveData);
        GameManager.OnStartGameState += GameManager_OnStartGameState;
    }

    private void OnDestroy() => GameManager.OnStartGameState -= GameManager_OnStartGameState;
    private void GameManager_OnStartGameState(GameManager.GameState gameState)
    {
        Debug.Log($"GameManager_OnStartGameState called with state: {gameState}");

        if (gameState != GameManager.GameState.UPGRADE)
            return;

 
        Debug.Log($"Starting refresh. AllSkillNodes count: {allSkillNodes.Count}");
        RefreshAllNodes();
    }

    private IEnumerator RefreshNodesNextFrame()
    {
        Debug.Log("RefreshNodesNextFrame started");
        yield return null;
        Debug.Log("About to call RefreshAllNodes");
        RefreshAllNodes();
        Debug.Log("RefreshAllNodes completed");
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
        leveledSkills[paintSpawner.ChanceOfSpawningSplitPerLevel.SkillID] = paintSpawner.ChanceOfSpawningSplitPerLevel;
        leveledSkills[paintSpawner.SpawnTimeIntervalPerLevel.SkillID] = paintSpawner.SpawnTimeIntervalPerLevel;
        leveledSkills[paintSpawner.CountPerSpawnPerLevel.SkillID] = paintSpawner.CountPerSpawnPerLevel;
        leveledSkills[paintSpawner.MaxSpawnCountPerLevel.SkillID] = paintSpawner.MaxSpawnCountPerLevel;
        leveledSkills[paintSpawner.InitialCountPerLevel.SkillID] = paintSpawner.InitialCountPerLevel;
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
        leveledSkills[currencyManager.CurrencyMultiplierPerLevel.SkillID] = currencyManager.CurrencyMultiplierPerLevel;
        leveledSkills[magneterManager.AttractionForcePerLevel.SkillID] = magneterManager.AttractionForcePerLevel;
        leveledSkills[magneterManager.AttractionRadiusPerLevel.SkillID] = magneterManager.AttractionRadiusPerLevel;
        leveledSkills[magneterManager.OrbitRadiusPerLevel.SkillID] = magneterManager.OrbitRadiusPerLevel;
        leveledSkills[magneterManager.OrbitSpeedPerLevel.SkillID] = magneterManager.OrbitSpeedPerLevel;
        leveledSkills[magneterManager.MagneterCountPerLevel.SkillID] = magneterManager.MagneterCountPerLevel;
        leveledSkills[splitter.SplitCountPerLevel.SkillID] = splitter.SplitCountPerLevel;
        leveledSkills[vacuumer.VacuumForcePerLevel.SkillID] = vacuumer.VacuumForcePerLevel;
        leveledSkills[vacuumer.VacuumRangePerLevel.SkillID] = vacuumer.VacuumRangePerLevel;
    
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
        paintSpawner.ChanceOfSpawningSplitPerLevel.Initialize();
        paintSpawner.SpawnTimeIntervalPerLevel.Initialize();
        paintSpawner.CountPerSpawnPerLevel.Initialize();
        paintSpawner.MaxSpawnCountPerLevel.Initialize();
        paintSpawner.InitialCountPerLevel.Initialize();
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
        currencyManager.CurrencyMultiplierPerLevel.Initialize();
        magneterManager.AttractionForcePerLevel.Initialize();
        magneterManager.AttractionRadiusPerLevel.Initialize();
        magneterManager.OrbitSpeedPerLevel.Initialize();
        magneterManager.OrbitRadiusPerLevel.Initialize();
        magneterManager.MagneterCountPerLevel.Initialize();
        splitter.SplitCountPerLevel.Initialize();
        vacuumer.VacuumForcePerLevel.Initialize();
        vacuumer.VacuumRangePerLevel.Initialize();
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

    public int GetSkillLevel(string skillID)
    {
        if (leveledSkills.TryGetValue(skillID, out var skillLevelData))
            return skillLevelData.CurrentLevel;
        return 0;
    }

    public ISkillLevelData GetSkillLevelData(string skillID)
    {
        if (leveledSkills.TryGetValue(skillID, out var skillLevelData))
            return skillLevelData;
        return null;
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
       
        if (levelRequirement != null && levelRequirement.RequiredSkills != null)
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

        int skillPointCost = skillData.GetSkillPointCostForLevel(targetLevel);
        if (skillPointCost > 0 && !SkillPointManager.Instance.CanAfford(skillPointCost))
            return false;

        return true;
    }

    public void TryLevelUpSkill(SkillDataBase skillData)
    {
        if (!leveledSkills.TryGetValue(skillData.SkillID, out var skillLevelData))
            return;

        int targetLevel = skillLevelData.CurrentLevel + 1;
        TryLevelUpSkillToLevel(skillData, targetLevel);
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

        int skillPointCost = skillData.GetSkillPointCostForLevel(targetLevel);
        if (skillPointCost > 0)
            SkillPointManager.Instance.TrySpendSkillPoints(skillPointCost);

        skillLevelData.LevelUp();
        totalUpgradesBought++;

        RefreshAllNodes();

        string costLog = $"[Cost: {cost} currency";
        if (skillPointCost > 0)
            costLog += $", {skillPointCost} SP";
        costLog += "]";

        logger.Log($"Leveled up {skillData.SkillName} → Level {skillLevelData.CurrentLevel} (Value: {skillLevelData.GetCurrentLevelData():F2}) " +
            $"{costLog}\nTotal Upgrades bought: {totalUpgradesBought}", this);
        Save();
    }
    public void RefundAndResetSkillTree()
    {
        int totalCurrencyRefund = 0;
        int totalSkillPointsRefund = 0;

        CalculateRefundCost(ref totalCurrencyRefund, ref totalSkillPointsRefund);
        ResetSkillTree();
        RefundCurrency(totalCurrencyRefund);
        RefundSkillPoints(totalSkillPointsRefund);

        Save();
        logger.Log($"Skill tree reset complete. Refunded: {totalCurrencyRefund} currency, {totalSkillPointsRefund} skill points", this);
    }

    private void CalculateRefundCost(ref int totalCurrencyRefund, ref int totalSkillPointsRefund)
    {
        foreach (var kvp in leveledSkills)
        {
            ISkillLevelData skillLevelData = kvp.Value;
            SkillDataBase skillData = GetSkillDataBase(kvp.Key);

            if (skillData == null)
                continue;

            for (int level = 1; level <= skillLevelData.CurrentLevel; level++)
            {
                totalCurrencyRefund += skillData.GetCostForLevel(level);
                totalSkillPointsRefund += skillData.GetSkillPointCostForLevel(level);
            }
        }
    }

    private void RefundSkillPoints(int totalSkillPointsRefund)
    {
        if (totalSkillPointsRefund > 0)
        {
            SkillPointManager.Instance.AddSkillPoints(totalSkillPointsRefund);
            logger.Log($"Refunded {totalSkillPointsRefund} skill points", this);
        }
    }

    private void RefundCurrency(int totalCurrencyRefund)
    {
        if (totalCurrencyRefund > 0)
        {
            CurrencyManager.Instance.AddCurrency(totalCurrencyRefund);
            logger.Log($"Refunded {totalCurrencyRefund} currency", this);
        }
    }

    private SkillDataBase GetSkillDataBase(string skillID)
    {
        foreach (var node in allSkillNodes)
        {
            if (node != null && node.SkillDataBase != null && node.SkillDataBase.SkillID == skillID)
                return node.SkillDataBase;
        }
        return null;
    }

    public bool IsSkillUnlocked(string skillID) => leveledSkills[skillID].IsUnlocked;

    public void RefreshAllNodes()
    {
        Debug.Log($"RefreshAllNodes called. Node count: {allSkillNodes.Count}");
        foreach (var node in allSkillNodes)
        {
            if (node != null)
            {
                Debug.Log($"Updating node: {node.SkillDataBase?.SkillName}, State before: {node.CurrentState}");
                node.UpdateVisuals();
                Debug.Log($"State after: {node.CurrentState}");
            }
        }
    }

    public void ResetSkillTree()
    {
        foreach (var skill in leveledSkills.Values)
        {
            skill.Initialize();
        }
        totalUpgradesBought = 0;
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

        totalUpgradesBought = 0;

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
            totalUpgradesBought += kvp.Value;
        }
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