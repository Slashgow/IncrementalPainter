using System;
using UnityEngine;
using UnityTimer;

public class AutoClicker : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private inkolorgames.Logger logger;

    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> clickTimerIntervalPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> ClickTimerIntervalPerLevel => clickTimerIntervalPerLevel;
    public float ClickTimerInterval => clickTimerIntervalPerLevel.GetCurrentLevelData();

    [SerializeField] private bool useRealTime = false;
    public bool UseRealTime => useRealTime;

    public event Action<Vector3> OnClick;
    public event Action OnAutoClickStarted;
    public event Action OnAutoClickFinished;
    private Timer clickTimer;

    private void Awake()
    {
        clickTimerIntervalPerLevel.OnLevelUp += HandleLevelUp;
    }

    private void Start()
    {
        StartAutoClicking();
    }

    private void OnDestroy()
    {
        StopAutoClicking();
        clickTimerIntervalPerLevel.OnLevelUp -= HandleLevelUp;
    }

    private void HandleLevelUp()
    {
        if (clickTimer != null && !clickTimer.isDone)
        {
            StopAutoClicking();
            StartAutoClicking();
        }
    }

    public void StartAutoClicking()
    {
        clickTimer?.Cancel();
        clickTimer = Timer.Register(ClickTimerInterval, onComplete: SimulateClick, isLooped: true, useRealTime: useRealTime);
        OnAutoClickStarted?.Invoke();
    }

    public void StopAutoClicking()
    {
        clickTimer?.Cancel();
        OnAutoClickFinished?.Invoke();
    }

    private void SimulateClick()
    {
        logger.Log($"AutoClicker: SimulateClick at {Input.mousePosition}", this);
        OnClick?.Invoke(targetCamera.ScreenToWorldPoint(Input.mousePosition));
    }
}
