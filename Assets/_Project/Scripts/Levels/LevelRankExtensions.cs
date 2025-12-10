public static class LevelRankExtensions
{
    public static string GetDisplayName(this LevelRank rank)
    {
        return rank switch
        {
            LevelRank.S => "S",
            LevelRank.A => "A",
            LevelRank.B => "B",
            LevelRank.C => "C",
            LevelRank.D => "D",
            LevelRank.None => "Unranked",
            _ => "Unknown"
        };
    }

    public static UnityEngine.Color GetRankColor(this LevelRank rank)
    {
        return rank switch
        {
            LevelRank.S => new UnityEngine.Color(1f, 0.84f, 0f),      // Gold
            LevelRank.A => new UnityEngine.Color(0.75f, 0.75f, 0.75f), // Silver
            LevelRank.B => new UnityEngine.Color(0.8f, 0.5f, 0.2f),    // Bronze
            LevelRank.C => new UnityEngine.Color(0.6f, 0.6f, 0.6f),    // Gray
            LevelRank.D => new UnityEngine.Color(0.4f, 0.3f, 0.2f),    // Brown
            _ => UnityEngine.Color.white
        };
    }
}
