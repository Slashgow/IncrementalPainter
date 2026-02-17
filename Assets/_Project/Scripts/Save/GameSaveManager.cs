using System.Collections.Generic;
using System.IO;
using inkolorgames;
using Newtonsoft.Json;
using PaintCore;
using SaveUtility;
using UnityEngine;

public class GameSaveManager : PersistentMonoSingleton<GameSaveManager>
{
    [SerializeField] private inkolorgames.Logger logger;

    private GameSaveData gameSaveData;
    public Dictionary<string, LevelSaveData> GetAllLevelSaves() => gameSaveData.levels;

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
        string json = JsonConvert.SerializeObject(gameSaveData, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore} );
        File.WriteAllText(fullPath, json);
    }

    public void SaveLevelData(bool isDone, float completionPercentage, string author, string title, bool isUnlocked = false, 
        int daysToComplete = -1, LevelRank bestRank = LevelRank.None, bool hasClaimedReward = false, LevelStats levelStats = null)
    {
        gameSaveData.levels.TryGetValue(SavePath.GetLevelID(author, title), out LevelSaveData existingEntry);

        if (existingEntry != null)
        {
            logger.Log($"Saving existing {SavePath.GetLevelID(author, title)}", this);
            existingEntry.isDone = isDone;
            existingEntry.completionRatio = completionPercentage;
            existingEntry.isUnlocked = isUnlocked;
            existingEntry.daysToComplete = daysToComplete == - 1 ? existingEntry.daysToComplete : daysToComplete;
            existingEntry.bestRank = bestRank == LevelRank.None ? existingEntry.bestRank : bestRank;
            existingEntry.levelStats = levelStats ?? existingEntry.levelStats;
        }
        else
        {
            logger.Log($"Saving unexisting {SavePath.GetLevelID(author, title)}", this);
            gameSaveData.levels.Add(SavePath.GetLevelID(author, title), 
                new LevelSaveData(isDone, completionPercentage, author, title, isUnlocked, daysToComplete, bestRank, 
                hasClaimedReward, levelStats));
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

    public void ClearLevelProgress(string author, string title)
    {
        LevelSaveData saveData = LoadLevelData(author, title);
        SaveLevelData(false, 0, author, title, saveData.isUnlocked, 0, saveData.bestRank, false, new LevelStats());
        CwCommon.ClearSave(SavePath.GetLevelID(author, title), SavePath.SaveFolderSprites);
    }

    public void SaveSkillTree(SkillTreeSaveData skillTreeData)
    {
        gameSaveData.skillTree = skillTreeData;
        SaveGameData();
        logger.Log("Skill tree saved", this);
    }

    public SkillTreeSaveData LoadSkillTree() => gameSaveData.skillTree ?? new SkillTreeSaveData();
    public void SaveSkillPoints(int skillPoints)
    {
        gameSaveData.skillPoints = skillPoints;
        SaveGameData();
        logger.Log($"Saved {skillPoints} skill points", this);
    }

    public int LoadSkillPoints() => gameSaveData.skillPoints;

    public void SaveCurrency(int currency)
    {
        gameSaveData.currency = currency;
        SaveGameData();
        logger.Log($"Saved {currency} currency", this);
    }

    public int LoadCurrency() => gameSaveData.currency;

    public void SaveColorThemes(ColorThemeSaveData colorThemeData)
    {
        gameSaveData.colorThemes = colorThemeData;
        SaveGameData();
        logger.Log("Color themes saved", this);
    }

    public ColorThemeSaveData LoadColorThemes()
    {
        if (gameSaveData.colorThemes == null)
        {
            gameSaveData.colorThemes = new ColorThemeSaveData();
        }
        return gameSaveData.colorThemes;
    }

    public BrushSaveData LoadBrushData() 
    {
        if (gameSaveData.brushSaveData == null)
        {
            gameSaveData.brushSaveData = new BrushSaveData();
        }
        return gameSaveData.brushSaveData;
    }
    public void SaveBrushData(BrushSaveData data) 
    {
        gameSaveData.brushSaveData = data;
        SaveGameData();
        logger.Log("Brush data saved", this);
    }

    public TamponSaveData LoadTamponData()
    {
        if (gameSaveData.tamponSaveData == null)
        {
            gameSaveData.tamponSaveData = new TamponSaveData();
        }
        return gameSaveData.tamponSaveData;
    }

    public void SaveTamponData(TamponSaveData data)
    {
        gameSaveData.tamponSaveData = data;
        SaveGameData();
        logger.Log("Tampon data saved", this);
    }

    public void SaveArtGalleryLayout(Dictionary<string, ArtGalleryPaintingSaveData> layout)
    {
        gameSaveData.artGalleryLayout = layout;
        SaveGameData();
        logger.Log($"Art gallery layout saved ({layout.Count} entries)", this);
    }

    public Dictionary<string, ArtGalleryPaintingSaveData> LoadArtGalleryLayout()
    {
        if (gameSaveData.artGalleryLayout == null)
            gameSaveData.artGalleryLayout = new Dictionary<string, ArtGalleryPaintingSaveData>();

        return gameSaveData.artGalleryLayout;
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

