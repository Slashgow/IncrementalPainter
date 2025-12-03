using System.Collections.Generic;

[System.Serializable]
public class SkillTreeSaveData
{
    public Dictionary<string, int> skillLevels = new Dictionary<string, int>();

    public SkillTreeSaveData() { }

    public SkillTreeSaveData(Dictionary<string, ISkillLevelData> leveledSkills)
    {
        foreach (var kvp in leveledSkills)
        {
            skillLevels[kvp.Key] = kvp.Value.CurrentLevel;
        }
    }
}
