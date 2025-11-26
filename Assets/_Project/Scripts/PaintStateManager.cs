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
    protected override void Awake()
    {
        base.Awake();
        GameManager.OnStartGameState += HandleStartGameState;
        SimpleDamageable.OnAnyDamageableDie += SimpleDamageable_OnAnyDamageableDie;
    }

    private void OnDestroy()
    {
        SimpleDamageable.OnAnyDamageableDie -= SimpleDamageable_OnAnyDamageableDie;
        if (GameManager.HasInstance)
            GameManager.OnStartGameState -= HandleStartGameState;
    }
    private void HandleStartGameState(GameManager.GameState state)
    {
        if(state != GameManager.GameState.PAINT)
            return;

        countDownPaintState = new Countdown(TimeOfPaintStatePerLevel.GetCurrentLevelData(), 1f, SwitchStateToDaySummary);
        countDownPaintState.Start(this);
        OnStartPaintState?.Invoke();
    }

    public void SwitchStateToDaySummary()
    {
        GameManager.Instance.SwitchState(GameManager.GameState.DAY_SUMMARY);
    }

    private void SimpleDamageable_OnAnyDamageableDie(Vector3 worldPosition, Color color) => TryIncreasingTimer();
    private void TryIncreasingTimer()
    {
        if (countDownPaintState == null)
            return;

        if (LuckUtility.RollLuck(chanceOfIncreasingTimerPerLevel.GetCurrentLevelData()))
        {
            countDownPaintState.AddTime(timeToAddOnIncreasePerLevel.GetCurrentLevelData());
        }
    }
}
