using System;
using inkolorgames;
using UnityEngine;

public class PaintStateManager : MonoSingleton<PaintStateManager>
{
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> timeOfPaintStatePerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> TimeOfPaintStatePerLevel => timeOfPaintStatePerLevel;

    private Countdown countDownPaintState;
    public Countdown CountdownPaintState => countDownPaintState;

    public static event Action OnStartPaintState;
    protected override void Awake()
    {
        base.Awake();
        GameManager.OnStartGameState += HandleStartGameState;
    }

    private void OnDestroy()
    {
        if(GameManager.HasInstance)
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
}
