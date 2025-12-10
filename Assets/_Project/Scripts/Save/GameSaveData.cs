using System;
using System.Collections.Generic;

[Serializable]
public class GameSaveData
{
    public Dictionary<string, LevelSaveData> levels = new();
    public SkillTreeSaveData skillTree;
    public int skillPoints = 0;

    public GameSaveData()
    {
        levels = new Dictionary<string, LevelSaveData>();
        skillTree = new SkillTreeSaveData();
        skillPoints = 0;
    }
}
