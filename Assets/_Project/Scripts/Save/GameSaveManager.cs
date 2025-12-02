using System.IO;
using inkolorgames;
using Newtonsoft.Json;
using SaveUtility;
using UnityEngine;

public class GameSaveManager : PersistentMonoSingleton<GameSaveManager>
{
    [SerializeField] private inkolorgames.Logger logger;

    private GameSaveData gameSaveData;

    protected override void Awake()
    {
        base.Awake();

        LoadGameData();
    }

    private void LoadGameData()
    {
        string fullPath = SavePath.FullPathSaveFile;

        if (File.Exists(fullPath))
        {
            string json = File.ReadAllText(fullPath);
            gameSaveData = JsonConvert.DeserializeObject<GameSaveData>(json);
        }
        else
        {
            gameSaveData = new GameSaveData();
        }
    }


    private void SaveGameData()
    {
        string fullPath = SavePath.FullPathSaveFile;
        string json = JsonConvert.SerializeObject(gameSaveData);
        File.WriteAllText(fullPath, json);
    }

    public void SaveLevelData(bool isDone, float completionPercentage, string author, string title, bool isUnlocked = false)
    {
        gameSaveData.levels.TryGetValue(SavePath.GetLevelID(author, title), out LevelSaveData existingEntry);

        if (existingEntry != null)
        {
            logger.Log($"Saving existing {SavePath.GetLevelID(author, title)}", this);
            existingEntry.isDone = isDone;
            existingEntry.completionRatio = completionPercentage;
            existingEntry.isUnlocked = isUnlocked;
        }
        else
        {
            logger.Log($"Saving unexisting {SavePath.GetLevelID(author, title)}", this);
            gameSaveData.levels.Add(SavePath.GetLevelID(author, title), 
                new LevelSaveData(isDone, completionPercentage, author, title, isUnlocked));
        }

        SaveGameData();
    }

    public LevelSaveData LoadLevelData(string author, string title)
    {
        gameSaveData.levels.TryGetValue(SavePath.GetLevelID(author, title), out LevelSaveData entry);

        if (entry != null)
        {
            logger.Log($"Loading level {entry.ID}", this);
            return entry;
        }

        logger.Log($"Could not load level {SavePath.GetLevelID(author, title)} \n" +
            $"creating new entry with default values", this);
        return new LevelSaveData(false, 0f, author, title);
    }

    public bool LevelSaveExists(string author, string title) => gameSaveData.levels.ContainsKey(SavePath.GetLevelID(author, title));
    public void DeleteLevelSave(string author, string title)
    {
        gameSaveData.levels.Remove(SavePath.GetLevelID(author, title));
        SaveGameData();
    }

    public void ClearAllSaves()
    {
        gameSaveData = new GameSaveData();
        SaveGameData();
        if (Directory.Exists(SavePath.SaveFolderSprites))
        {
            DirectoryInfo di = new DirectoryInfo(SavePath.SaveFolderSprites);
            foreach (FileInfo file in di.GetFiles("*.json"))
            {
                file.Delete();
            }
        }
    }
}

