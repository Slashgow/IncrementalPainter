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

    public bool IsPause { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        IsPause = true;
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
        Resume();
        LevelManager.Instance.SetNextLevelAsCurrentLevel();
        SceneLoader.Instance.ReloadSceneAsync();
    }

    public void Pause()
    {
        Debug.Log("try pause");
        if (IsPause)
            return;

        Debug.Log("pause");
        IsPause = true;
        Time.timeScale = 0f;
        CurrentPauseState = PauseState.PAUSE;
        OnPause?.Invoke();
    }

    public void Resume()
    {
        Debug.Log("Try Resume");
        if (CurrentGameState != GameState.PAINT)
            return;

        if (!IsPause)
            return;

        Debug.Log("Resume");
        IsPause = false;
        Time.timeScale = 1f;
        CurrentPauseState = PauseState.PLAY;
        OnResume?.Invoke();
    }
}
