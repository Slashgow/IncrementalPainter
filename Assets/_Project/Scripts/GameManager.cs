using System;
using inkolorgames;
using UnityEngine;
using UnityTimer;
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
        GALLERY,
        BOSS_INTRODUCTION,
    }

    [SerializeField] private GameState startingGameState;
    [SerializeField, Range(0f,5f)] private float bossIntroductionDuration = 3f;

    public GameState CurrentGameState { get; private set; }
    public static PauseState CurrentPauseState { get; private set; }
    public static event Action<GameState> OnStartGameState;
    public static event Action<GameState> OnEndGameState;
    public static event Action OnPause, OnResume;

    public bool IsPause { get; private set; }
    private Timer bossIntroTimer;

    protected override void Awake()
    {
        base.Awake();
        IsPause = true;
        CurrentPauseState = PauseState.PLAY;
        LevelManager.Instance.LoadCurrentLevel();
    }

    private void Start()
    {
        if (LevelManager.Instance.CurrentLevelData != null && LevelManager.Instance.CurrentLevelData.IsBossLevel)
            SwitchState(GameState.BOSS_INTRODUCTION);
        else
            SwitchState(startingGameState);
    }

    public void SwitchState(GameState targetGameState)
    {
        Debug.Log($"{targetGameState}");
        OnEndGameState?.Invoke(CurrentGameState);
        CurrentGameState = targetGameState;
        OnStartGameState?.Invoke(CurrentGameState);

        HandlePause();
        HandleBossIntroduction();
    }

    private void HandlePause()
    {
        if (CurrentGameState != GameState.PAINT)
            Pause();
        if (CurrentGameState == GameState.PAINT)
            Resume();
    }
    private void HandleBossIntroduction()
    {
        if (CurrentGameState == GameState.BOSS_INTRODUCTION)
        {
            bossIntroTimer?.Cancel();
            bossIntroTimer = Timer.Register(bossIntroductionDuration, onComplete: () => SwitchState(startingGameState), useRealTime: true);
        }
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
        if (IsPause)
            return;

        IsPause = true;
        Time.timeScale = 0f;
        CurrentPauseState = PauseState.PAUSE;
        OnPause?.Invoke();
    }

    public void Resume()
    {
        if (CurrentGameState != GameState.PAINT)
            return;

        if (!IsPause)
            return;

        IsPause = false;
        Time.timeScale = 1f;
        CurrentPauseState = PauseState.PLAY;
        OnResume?.Invoke();
    }

    private void OnDestroy()
    {
        bossIntroTimer?.Cancel();
    }
}
