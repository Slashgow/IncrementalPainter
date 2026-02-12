using System;
using System.Collections.Generic;

[Serializable]
public class GameSaveData
{
    public Dictionary<string, LevelSaveData> levels = new();
    public SkillTreeSaveData skillTree;
    public int skillPoints = 0;
    public int currency = 0;
    public ColorThemeSaveData colorThemes;
    public BrushSaveData brushSaveData;

    public GameSaveData()
    {
        levels = new Dictionary<string, LevelSaveData>();
        skillTree = new SkillTreeSaveData();
        colorThemes = new ColorThemeSaveData();
        brushSaveData = new BrushSaveData();
        skillPoints = 0;
        currency = 0;
    }
}
