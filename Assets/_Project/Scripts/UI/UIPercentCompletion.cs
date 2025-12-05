using PaintCore;

public class UIPercentCompletion : CwChangeCounterText
{
    private void Start()
    {
        counters.Add(LevelManager.Instance.CurrentLevelInstance.ColorChangeCounter);
    }
}
