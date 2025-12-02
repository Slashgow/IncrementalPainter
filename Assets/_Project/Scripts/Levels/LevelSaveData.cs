using System;
using SaveUtility;

[Serializable]
public class LevelSaveData
{
    public string ID => SavePath.GetLevelID(author, title);

    public bool isDone;
    public float completionRatio;
    public string author;
    public string title;

    public LevelSaveData(bool isDone, float completionRatio, string author, string title)
    {
        this.isDone = isDone;
        this.completionRatio = completionRatio;
        this.author = author;
        this.title = title;
    }
}
