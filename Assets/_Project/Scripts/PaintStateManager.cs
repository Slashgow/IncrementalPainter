using System;
using inkolorgames;
using UnityEngine;

public class PaintStateManager : MonoSingleton<PaintStateManager>
{
    [SerializeField] private inkolorgames.Logger logger;

    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> timeOfPaintStatePerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> TimeOfPaintStatePerLevel => timeOfPaintStatePerLevel;

    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> chanceOfIncreasingTimerPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> ChanceOfIncreasingTimerPerLevel => chanceOfIncreasingTimerPerLevel;

    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> timeToAddOnIncreasePerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> TimeToAddOnIncreasePerLevel => timeToAddOnIncreasePerLevel;

    private Countdown countDownPaintState;
    public Countdown CountdownPaintState => countDownPaintState;

    public static event Action OnStartPaintState;
    public static event Action<float> OnAddedTimeToCountdown;
    public static event Action<float> OnRemovedTimeFromCountdown;
    protected override void Awake()
    {
        base.Awake();
        GameManager.OnStartGameState += HandleStartGameState;
        SimpleDamageable.OnAnyDamageableDie += SimpleDamageable_OnAnyDamageableDie;
        LevelManager.OnEndLevel += LevelManager_OnEndLevel;
    }

    private void OnDestroy()
    {
        SimpleDamageable.OnAnyDamageableDie -= SimpleDamageable_OnAnyDamageableDie;
        GameManager.OnStartGameState -= HandleStartGameState;
        LevelManager.OnEndLevel -= LevelManager_OnEndLevel;
    }
    private void HandleStartGameState(GameManager.GameState state)
    {
        if(state != GameManager.GameState.PAINT)
            return;

        countDownPaintState = new Countdown(TimeOfPaintStatePerLevel.GetCurrentLevelData(), 1f, SwitchStateToDaySummary);
        countDownPaintState.Start(this);
        OnStartPaintState?.Invoke();
    }

    private void LevelManager_OnEndLevel()
    {
        SwitchStateToDaySummary();
    }

    public void SwitchStateToDaySummary()
    {
        countDownPaintState.Cancel();
        GameManager.Instance.SwitchState(GameManager.GameState.DAY_SUMMARY);
    }

    private void SimpleDamageable_OnAnyDamageableDie(Vector3 worldPosition, Color color, float scale) => TryIncreasingTimer();
    private void TryIncreasingTimer()
    {
        if (countDownPaintState == null)
            return;

        if (LuckUtility.RollLuck(chanceOfIncreasingTimerPerLevel.GetCurrentLevelData()))
        {
            countDownPaintState.AddTime(timeToAddOnIncreasePerLevel.GetCurrentLevelData());
            OnAddedTimeToCountdown?.Invoke(timeToAddOnIncreasePerLevel.GetCurrentLevelData());
        }
    }

    public void AddTimeToCountdown(float timeToAdd)
    {
        countDownPaintState.AddTime(timeToAdd);
        OnAddedTimeToCountdown?.Invoke(timeToAdd);
    }

    public void RemoveTimeFromCountdown(float timeToRemove)
    {
        countDownPaintState.ReduceTime(timeToRemove);
        OnRemovedTimeFromCountdown?.Invoke(timeToRemove);
    }
}
