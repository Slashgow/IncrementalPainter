using System;
using inkolorgames;
using UnityEngine;
public class GameManager : MonoSingleton<GameManager>
{
    [Serializable]
    public enum GameState
    {
        PAINT,
        DAY_SUMMARY,
        UPGRADE,
        GALLERY
    }

    [SerializeField] private GameState startingGameState;

    public GameState CurrentGameState { get; private set; }

    public static event Action<GameState> OnStartGameState;
    public static event Action<GameState> OnEndGameState;

    protected override void Awake()
    {
        base.Awake();
        LevelManager.Instance.LoadCurrentLevel();
    }

    private void Start()
    {
        SwitchState(startingGameState);
    }

    public void SwitchState(GameState targetGameState)
    {
        OnEndGameState?.Invoke(CurrentGameState);
        CurrentGameState = targetGameState;
        OnStartGameState?.Invoke(CurrentGameState);
    }

    public void StartPaintState() => SwitchState(GameState.PAINT);
    public void StartUpgradeState() => SwitchState(GameState.UPGRADE);
}
