using System;
using inkolorgames;
using UnityEngine;
public class GameManager : MonoSingleton<GameManager>
{
    public enum PauseState
    {
        PAUSE,
        PLAY
    }

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
    public static PauseState CurrentPauseState { get; private set; }
    public static event Action<GameState> OnStartGameState;
    public static event Action<GameState> OnEndGameState;
    public static event Action OnPause, OnResume;

    protected override void Awake()
    {
        base.Awake();
        CurrentPauseState = PauseState.PLAY;
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

        HandlePause();
    }

    private void HandlePause()
    {
        if (CurrentGameState != GameState.PAINT)
            Pause();
        if (CurrentGameState == GameState.PAINT)
            Resume();
    }

    public void StartPaintState() => SwitchState(GameState.PAINT);
    public void StartUpgradeState() => SwitchState(GameState.UPGRADE);

    public void StartNextLevel()
    {
        LevelManager.Instance.SetNextLevelAsCurrentLevel();
        SceneLoader.Instance.ReloadSceneAsync();
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        CurrentPauseState = PauseState.PAUSE;
        OnPause?.Invoke();
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        CurrentPauseState = PauseState.PLAY;
        OnResume?.Invoke();
    }
}
