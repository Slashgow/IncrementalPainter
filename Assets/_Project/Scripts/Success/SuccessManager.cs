using System;
using System.Collections.Generic;
using System.Linq;
using inkolorgames;
using UnityEngine;

public class SuccessManager : PersistentMonoSingleton<SuccessManager>
{
    [SerializeField] private bool listenToSuccessCompletion = true;

    [Header("References")]
    [SerializeField] private SkillDataBase nukeSkillDataBase;

    [Header("Success")]
    [SerializeField] private SuccessData finishASessionSuccessData;
    [SerializeField] private SuccessData startSessionWithXAmountOfPaintSuccessData;
    [SerializeField] private SuccessData inflictXAmountOfDamageInOneHit; 
    [SerializeField] private SuccessData getXCurrencyInOneSessionSuccessData;
    [SerializeField] private SuccessData buyXUpgradeSuccessData;
    [SerializeField] private SuccessData unlockXColorPaletteSuccessData;
    [SerializeField] private SuccessData unlockXTamponSuccessData;
    //[SerializeField] private SuccessData unlockXBrushSuccessData;   // TO DO
    [SerializeField] private SuccessData finishRankSAllPaintings;  
    [SerializeField] private SuccessData doXDamageInOneSessionSuccessData;
    [SerializeField] private SuccessData unlockNukeSuccessData;


    private SuccessStatData successStatData;
    public SuccessStatData SuccessStatData => successStatData;

    private List<SuccessData> allSuccessData;
    public List<SuccessData > AllSuccessData => allSuccessData;

    protected override void Awake()
    {
        base.Awake();
        if(allSuccessData == null)
            InitializeSuccessList();
    }
    public void LoadDefault()
    {
        InitializeSuccessList();
        InitializeSuccessStatData();
    }

    public void LoadFromSuccessSaveData(SuccessSaveData successSaveData)
    {
        InitializeSuccessList();

        if (successSaveData.successCompletionStates != null &&
            successSaveData.successCompletionStates.Count == allSuccessData.Count)
        {
            for (int i = 0; i < allSuccessData.Count; i++)
            {
                allSuccessData[i].isDone = successSaveData.successCompletionStates[i];
            }
        }

        successStatData = new SuccessStatData(successSaveData.successStatData);
    }

    private void CheckStat()
    {
        LevelManager_OnEndLevel();
        OnUnlockTampon(null);
        OnUnlockColorPalette(null);
    }

    public void ResetSuccess()
    {
        InitializeSuccessStatData();
        allSuccessData.ForEach(successData => successData.isDone = false);
    }

    private void InitializeSuccessStatData() => successStatData = new SuccessStatData();

    private void InitializeSuccessList()
    {
        allSuccessData = new List<SuccessData>
        {
            finishASessionSuccessData,
            startSessionWithXAmountOfPaintSuccessData,
            inflictXAmountOfDamageInOneHit,
            getXCurrencyInOneSessionSuccessData,
            buyXUpgradeSuccessData,
            unlockXColorPaletteSuccessData,
            unlockXTamponSuccessData,
            //unlockXBrushSuccessData,
            finishRankSAllPaintings,
            doXDamageInOneSessionSuccessData,
            unlockNukeSuccessData
        };
    }

    public List<SuccessData> GetCompletedSuccessData()
    {
        List<SuccessData> completedSuccess = new List<SuccessData>();

        foreach (SuccessData success in allSuccessData)
        {
            if (success.isDone)
                completedSuccess.Add(success);
        }

        return completedSuccess;
    }

    public int GetCompletedSuccessCount() => GetCompletedSuccessData().Count;

    public List<SuccessData> GetUncompletedSuccessData()
    {
        List<SuccessData> uncompletedSuccess = new List<SuccessData>();

        foreach (SuccessData success in allSuccessData)
        {
            if (!success.isDone)
                uncompletedSuccess.Add(success);
        }

        return uncompletedSuccess;
    }
    public float GetCompletionPercentage()
    {
        if (allSuccessData.Count == 0) return 0f;

        int completedCount = GetCompletedSuccessData().Count;
        return (float)completedCount / allSuccessData.Count * 100f;
    }

    private void Start()
    {
        CheckStat();

        if (!listenToSuccessCompletion)
            return;

        PaintStateManager.OnEndPaintState += PaintStateManager_OnEndPaintState;
        PaintSpawner.OnFinishInitializeSpawning += PaintSpawner_OnFinishInitializeSpawning;
        SkillTreeManager.OnUpgradesBought += SkillTreeManager_OnUpgradesBought;
        ColorThemeUnlockManager.Instance.OnItemUnlocked += OnUnlockColorPalette;
        TamponUnlockManager.Instance.OnItemUnlocked += OnUnlockTampon;
        LevelManager.OnEndLevel += LevelManager_OnEndLevel;
        BaseDamageor.OnAnyDamageaorAttackOnce += BaseDamageor_OnAnyDamageaorAttackOnce;
    }

  

    private void OnDestroy()
    {
        if (!listenToSuccessCompletion)
            return;

        PaintStateManager.OnEndPaintState -= PaintStateManager_OnEndPaintState;
        PaintSpawner.OnFinishInitializeSpawning -= PaintSpawner_OnFinishInitializeSpawning;
        SkillTreeManager.OnUpgradesBought -= SkillTreeManager_OnUpgradesBought;

        if(ColorThemeUnlockManager.HasInstance)
            ColorThemeUnlockManager.Instance.OnItemUnlocked -= OnUnlockColorPalette;

        if(TamponUnlockManager.HasInstance)
            TamponUnlockManager.Instance.OnItemUnlocked -= OnUnlockTampon;

        LevelManager.OnEndLevel -= LevelManager_OnEndLevel;
        BaseDamageor.OnAnyDamageaorAttackOnce -= BaseDamageor_OnAnyDamageaorAttackOnce;
    }

    private void PaintStateManager_OnEndPaintState()
    {
        int currencyGainedThisDay = LevelStatsTracker.Instance.CurrentDayStats.CurrencyGained;
        float totalDamageDealtThisDay = LevelStatsTracker.Instance.CurrentDayStats.TotalDamageDealt;

        if (currencyGainedThisDay > successStatData.bestCurrencyGainedOneSession)
            successStatData.bestCurrencyGainedOneSession = currencyGainedThisDay;

        if (totalDamageDealtThisDay > successStatData.bestDamageOneSession)
            successStatData.bestDamageOneSession = Mathf.FloorToInt(totalDamageDealtThisDay);

        finishASessionSuccessData.Complete();

        if (currencyGainedThisDay >= getXCurrencyInOneSessionSuccessData.Value)
            getXCurrencyInOneSessionSuccessData.Complete();

        if (totalDamageDealtThisDay >= doXDamageInOneSessionSuccessData.Value)
            doXDamageInOneSessionSuccessData.Complete();
    }

    private void PaintSpawner_OnFinishInitializeSpawning(int spawnCount)
    {
        if (spawnCount > successStatData.bestNumberOfBlobAtSessionStart)
            successStatData.bestNumberOfBlobAtSessionStart = spawnCount;

        if (spawnCount >= startSessionWithXAmountOfPaintSuccessData.Value)
            startSessionWithXAmountOfPaintSuccessData.Complete();
    }

    private void SkillTreeManager_OnUpgradesBought(int totalUpgradesBought, string skillID)
    {
        successStatData.upgradeBought = totalUpgradesBought;

        if(totalUpgradesBought >= buyXUpgradeSuccessData.Value)
            buyXUpgradeSuccessData.Complete();

        if(nukeSkillDataBase.SkillID == skillID)
            unlockNukeSuccessData.Complete();
    }

    private void OnUnlockColorPalette(string id)
    {
        int unlockColorPaletteCount = ColorThemeUnlockManager.Instance.GetUnlockedItemCount();

        successStatData.countColorPaletteUnlocked = unlockColorPaletteCount;

        if (unlockXColorPaletteSuccessData.Value <= unlockColorPaletteCount)
            unlockXColorPaletteSuccessData.Complete();
    }

    private void OnUnlockTampon(string id)
    {
        int unlockTamponCount = TamponUnlockManager.Instance.GetUnlockedItemCount();

        successStatData.countTamponUnlocked = unlockTamponCount;

        if (unlockXTamponSuccessData.Value <= unlockTamponCount)
            unlockXTamponSuccessData.Complete();
    }

    private void LevelManager_OnEndLevel()
    {
        var levelsSaveData = GameSaveManager.Instance.GetAllLevelSaves();

        int rankSCount = levelsSaveData.Count(level => level.Value.bestRank == LevelRank.S);
        successStatData.paintRankSFinish = rankSCount;

        if (levelsSaveData.Any(levelSaveData => levelSaveData.Value.bestRank != LevelRank.S))
            return;

        finishRankSAllPaintings.Complete();
    }

    private void BaseDamageor_OnAnyDamageaorAttackOnce(float cummulateRawDamage)
    {
        if (cummulateRawDamage > successStatData.bestDamageSingleHit)
            successStatData.bestDamageSingleHit = Mathf.FloorToInt(cummulateRawDamage);

        if(cummulateRawDamage >= inflictXAmountOfDamageInOneHit.Value)
            inflictXAmountOfDamageInOneHit.Complete();
    }
}
