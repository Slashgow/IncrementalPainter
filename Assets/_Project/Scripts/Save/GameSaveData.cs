using System;
using System.Collections.Generic;

[Serializable]
public class GameSaveData
{
    public Dictionary<string, LevelSaveData> levels = new();
}
