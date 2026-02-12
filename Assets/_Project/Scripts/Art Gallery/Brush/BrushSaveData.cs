using System;
using System.Collections.Generic;

[Serializable]
public class BrushSaveData
{
    public List<string> unlockedBrushIds = new List<string>();
    public string currentBrushId;
}