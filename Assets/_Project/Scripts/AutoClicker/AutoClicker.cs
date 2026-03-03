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

    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> chanceOfBoostingRadiusPerLevel;
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> radiusSizeMultiplierPerLevel;
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> boostRadiusDurationPerLevel;
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> timeBetweenTryBoostRadiusPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> ChanceOfBoostingRadiusPerLevel => chanceOfBoostingRadiusPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> RadiusSizeMultiplierPerLevel => radiusSizeMultiplierPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> BoostRadiusDurationPerLevel => boostRadiusDurationPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> TimeBetweenTryBoostRadiusPerLevel => timeBetweenTryBoostRadiusPerLevel;
    public float ChanceOfBoostingRadius => chanceOfBoostingRadiusPerLevel.GetCurrentLevelData();
    public float RadiusSizeMultiplier => radiusSizeMultiplierPerLevel.GetCurrentLevelData();
    public float BoostRadiusDuration => boostRadiusDurationPerLevel.GetCurrentLevelData();
    public float TimeBetweenTryBoostRadius => timeBetweenTryBoostRadiusPerLevel.GetCurrentLevelData();

    [SerializeField] private bool useRealTime = false;
    public bool UseRealTime => useRealTime;

    public event Action<Vector3> OnClick;
    public event Action OnAutoClickStarted;
    public event Action OnAutoClickFinished;
    public event Action OnEnableAutoClicker;
    public event Action OnDisableAutoClicker;
    private Timer clickTimer;
    private bool isAutoClicking = false;
    private Timer tryBoostTimer;
    private Timer boostRadiusTimer;
    public event Action OnBoostRadiusStart;
    public event Action OnBoostRadiusEnd;
    public bool isBoostingRadius { get; private set; } = false;

    private void Awake()
    {
        clickTimerIntervalPerLevel.OnLevelUp += HandleLevelUp;
        clickTimerIntervalPerLevel.OnLevelDown += HandleLevelUp;
    }

    private void OnEnable() => autoClickerInput.OnToggleAutoclicker += AutoClickerInput_OnToggleAutoclicker;
    private void OnDisable() => autoClickerInput.OnToggleAutoclicker -= AutoClickerInput_OnToggleAutoclicker;

    private void Start()
    {
        StartAutoClicking();
        StartTryBoostRadius();
    }

    private void StartTryBoostRadius()
    {
        tryBoostTimer = Timer.Register(TimeBetweenTryBoostRadius, onComplete: TryBoostRadius, isLooped: true, useRealTime: false);
    }

    private void TryBoostRadius()
    {
        if (LuckUtility.RollLuck(ChanceOfBoostingRadius))
        {
            BoostRadius();
        }
    }

    private void BoostRadius()
    {
        tryBoostTimer?.Cancel();
        isBoostingRadius = true;
        OnBoostRadiusStart?.Invoke();
        boostRadiusTimer = Timer.Register(BoostRadiusDuration, onComplete: OnFinishBoostRadius, isLooped: false, useRealTime: false);
    }

    private void OnFinishBoostRadius()
    {
        isBoostingRadius = false;
        OnBoostRadiusEnd?.Invoke();
        StartTryBoostRadius();
    }

    private void OnDestroy()
    {
        StopAutoClicking();
        tryBoostTimer?.Cancel();
        boostRadiusTimer?.Cancel();
        clickTimerIntervalPerLevel.OnLevelUp -= HandleLevelUp;
        clickTimerIntervalPerLevel.OnLevelDown -= HandleLevelUp;
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
