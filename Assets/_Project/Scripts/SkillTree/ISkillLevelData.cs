public interface ISkillLevelData
{
    string SkillID { get; }
    int CurrentLevel { get; }
    bool IsUnlocked { get; }
    float GetCurrentLevelData();
    void Initialize();
    bool CanLevelUp();
    void LevelUp();
}
