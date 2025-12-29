using System;
using UnityEngine;
using UnityTimer;


public class AutoClicker : MonoBehaviour
{
    [SerializeField] private AutoClickerInput autoClickerInput;
    [SerializeField] private Camera targetCamera;
    [SerializeField] private inkolorgames.Logger logger;

    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> clickTimerIntervalPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> ClickTimerIntervalPerLevel => clickTimerIntervalPerLevel;
    public float ClickTimerInterval => clickTimerIntervalPerLevel.GetCurrentLevelData();

    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> toggleAutoclickerPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> ToggleAutoclickerPerLevel => toggleAutoclickerPerLevel;
    public bool IsAutoClickerToggleUnlocked => Mathf.FloorToInt(toggleAutoclickerPerLevel.GetCurrentLevelData()) > 0;

    [SerializeField] private bool useRealTime = false;
    public bool UseRealTime => useRealTime;

    public event Action<Vector3> OnClick;
    public event Action OnAutoClickStarted;
    public event Action OnAutoClickFinished;
    public event Action OnEnableAutoClicker;
    public event Action OnDisableAutoClicker;
    private Timer clickTimer;
    private bool isAutoClicking = false;

    private void Awake() => clickTimerIntervalPerLevel.OnLevelUp += HandleLevelUp;
    private void OnEnable() => autoClickerInput.OnToggleAutoclicker += AutoClickerInput_OnToggleAutoclicker;
    private void OnDisable() => autoClickerInput.OnToggleAutoclicker -= AutoClickerInput_OnToggleAutoclicker;

    private void Start() => StartAutoClicking();

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
        isAutoClicking = true;
        clickTimer = Timer.Register(ClickTimerInterval, onComplete: SimulateClick, isLooped: true, useRealTime: useRealTime);
        OnAutoClickStarted?.Invoke();
    }

    public void StopAutoClicking()
    {
        clickTimer?.Cancel();
        isAutoClicking = false;
        OnAutoClickFinished?.Invoke();
    }

    private void SimulateClick()
    {
        logger.Log($"AutoClicker: SimulateClick at {Input.mousePosition}", this);
        OnClick?.Invoke(targetCamera.ScreenToWorldPoint(Input.mousePosition));
    }

    private void AutoClickerInput_OnToggleAutoclicker()
    {
        if (!IsAutoClickerToggleUnlocked)
            return;

        if(isAutoClicking)
        {
            OnDisableAutoClicker?.Invoke();
            StopAutoClicking();
        }
        else
        {
            OnEnableAutoClicker?.Invoke();
            StartAutoClicking();
        }
    }
}
