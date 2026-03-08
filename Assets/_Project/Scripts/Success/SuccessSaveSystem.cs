using System;
using System.IO;
using inkolorgames;
using Newtonsoft.Json;
using SaveUtility;
using Steamworks;
using UnityEngine;

public class SuccessSaveSystem : PersistentMonoSingleton<SuccessSaveSystem>
{
    [SerializeField] private SuccessManager successManager;
    [SerializeField] private inkolorgames.Logger logger;

    protected override void Awake()
    {
        Load();
    }

    private void Start()
    {
        if (successManager == null)
            return;

        foreach (SuccessData success in successManager.AllSuccessData)
        {
            success.OnComplete += OnSuccessCompleted;
        }
    }

    private void OnDestroy()
    {
        if (successManager == null)
            return;

        foreach (SuccessData success in successManager.AllSuccessData)
        {
            success.OnComplete -= OnSuccessCompleted;
        }
    }

    private void OnSuccessCompleted(SuccessData successData) => Save();

    public void Save()
    {
        if (successManager == null)
        {
            logger.LogWarning("SuccessManager is null, cannot save success data", this);
            return;
        }

        try
        {
            SuccessStatData currentStats = successManager.SuccessStatData;

#if !UNITY_WEBGL
            if(SteamClient.IsValid)
                SteamIntegration.Instance.SetStats(currentStats);
            //SteamIntegration.Instance.StoreStats();
#endif

            SuccessSaveData saveData = new SuccessSaveData(currentStats, successManager.AllSuccessData);

            string json = JsonConvert.SerializeObject(saveData);

            string fullPath = SavePath.FullPathSuccessSaveFile;
            File.WriteAllText(fullPath, json);

            logger.Log($"Success data saved to {fullPath}", this);
        }
        catch (Exception e)
        {
            logger.LogError($"Failed to save success data: {e.Message}", this);
        }
    }

    public void Load()
    {
        try
        {
            string fullPath = SavePath.FullPathSuccessSaveFile;

            if (SavePath.SaveSuccessExists)
            {
                string json = File.ReadAllText(fullPath);
                SuccessSaveData saveData = JsonConvert.DeserializeObject<SuccessSaveData>(json);
            
                if (saveData != null)
                {
                    successManager.LoadFromSuccessSaveData(saveData);
                    logger.Log($"Success data loaded from {fullPath}", this);
                }
                else
                {
                    successManager.LoadDefault();
                    logger.LogWarning("Success save data is corrupted, using defaults", this);
                }
            }
            else
            {
                successManager.LoadDefault();
                logger.Log("No success save file found, using default values", this);
            }
        }
        catch (Exception e)
        {
            logger.LogError($"Failed to load success data: {e.Message}", this);
        }
    }

    public void ForceSave()
    {
        Save();
    }

    public void ClearSave()
    {
        SuccessSaveData successSaveData = new SuccessSaveData();
        successManager.ResetSuccess();
        Save();
    }
}