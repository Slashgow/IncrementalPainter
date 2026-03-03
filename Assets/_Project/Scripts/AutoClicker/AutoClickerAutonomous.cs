using UnityTimer;

public class AutoClickerAutonomous : AutoClicker
{

    override protected void Awake()
    {
        AutoClickerAutonomousManager.Instance.ClickTimerIntervalPerLevel.OnLevelUp += HandleLevelUp;
        AutoClickerAutonomousManager.Instance.ClickTimerIntervalPerLevel.OnLevelDown += HandleLevelUp;
    }

    protected override void OnDestroy()
    {
        StopAutoClicking();
        AutoClickerAutonomousManager.Instance.ClickTimerIntervalPerLevel.OnLevelUp -= HandleLevelUp;
        AutoClickerAutonomousManager.Instance.ClickTimerIntervalPerLevel.OnLevelDown -= HandleLevelUp;
    }

    public override void StartAutoClicking()
    {
        clickTimer?.Cancel();
        isAutoClicking = true;
        clickTimer = Timer.Register(AutoClickerAutonomousManager.Instance.ClickTimerInterval, onComplete: SimulateClick, isLooped: true, useRealTime: UseRealTime);
        NotifyAutoClickStarted();
    }

    protected override void SimulateClick()
    {
        NotifyClick(transform.position);
    }
}


