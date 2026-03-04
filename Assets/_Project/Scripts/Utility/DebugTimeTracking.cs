using System;
using System.Collections.Generic;
using inkolorgames;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugTimeTracking : PersistentMonoSingleton<DebugTimeTracking>
{
    // ?? GameState timing ????????????????????????????????????????????????????

    [Serializable]
    public struct StateTimeEntry
    {
        [HideInInspector] public string name;
        public GameManager.GameState state;
        public float seconds;
        [Range(0f, 1f)] public float percentage;
    }

    [SerializeField] private List<StateTimeEntry> _stateTimings = new();

    private readonly Dictionary<GameManager.GameState, float> _timePerState = new();
    private GameManager.GameState _currentState;
    private float _stateStartTime;
    private bool _isTrackingState;

    // ?? Main menu timing ????????????????????????????????????????????????????

    [Serializable]
    public struct MainMenuTimeEntry
    {
        public float seconds;
    }

    [SerializeField] private MainMenuTimeEntry _mainMenuTime;

    private float _menuStartTime;
    private bool _isTrackingMenu;

    // ?? Level timing ????????????????????????????????????????????????????????

    [Serializable]
    public struct LevelTimeEntry
    {
        [HideInInspector] public string name;
        public string levelTitle;
        public float seconds;
    }

    [SerializeField] private List<LevelTimeEntry> _levelTimings = new();

    private readonly Dictionary<string, float> _timePerLevel = new();
    private string _currentLevelTitle;
    private float _levelStartTime;
    private bool _isTrackingLevel;

    // ?? Lifecycle ???????????????????????????????????????????????????????????

    protected override void Awake()
    {
        base.Awake();

        foreach (GameManager.GameState state in Enum.GetValues(typeof(GameManager.GameState)))
        {
            _timePerState[state] = 0f;
            _stateTimings.Add(new StateTimeEntry { name = state.ToString(), state = state });
        }

        // Subscribe before first scene can be missed
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Awake fires on the starting scene — start menu tracking now if applicable
        if (SceneManager.GetActiveScene().buildIndex == 0)
            BeginMenuTracking();
    }

    private void OnEnable()
    {
        GameManager.OnStartGameState += OnStartGameState;
        GameManager.OnEndGameState += OnEndGameState;
        LevelManager.OnStartLevel += OnStartLevel;
        LevelManager.OnEndLevel += OnEndLevel;
    }

    private void OnDisable()
    {
        GameManager.OnStartGameState -= OnStartGameState;
        GameManager.OnEndGameState -= OnEndGameState;
        LevelManager.OnStartLevel -= OnStartLevel;
        LevelManager.OnEndLevel -= OnEndLevel;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update() => RefreshInspectorView();

    // ?? Scene tracking ??????????????????????????????????????????????????????

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0)
        {
            // Entered (or re-entered) the main menu
            BeginMenuTracking();
        }
        else if (_isTrackingMenu)
        {
            // Left the main menu
            StopMenuTracking();
        }
    }

    private void BeginMenuTracking()
    {
        _menuStartTime = Time.realtimeSinceStartup;
        _isTrackingMenu = true;
    }

    private void StopMenuTracking()
    {
        _mainMenuTime.seconds += Time.realtimeSinceStartup - _menuStartTime;
        _isTrackingMenu = false;
    }

    // ?? GameState tracking ??????????????????????????????????????????????????

    private void OnStartGameState(GameManager.GameState state)
    {
        _currentState = state;
        _stateStartTime = Time.time;
        _isTrackingState = true;
    }

    private void OnEndGameState(GameManager.GameState state)
    {
        if (!_isTrackingState) return;
        _timePerState[state] += Time.time - _stateStartTime;
        _isTrackingState = false;
    }

    // ?? Level tracking ??????????????????????????????????????????????????????

    private void OnStartLevel(Level level)
    {
        _currentLevelTitle = level.LevelData.LevelTitle;
        _levelStartTime = Time.time;
        _isTrackingLevel = true;
        _timePerLevel[_currentLevelTitle] = 0f; // reset on retry
    }

    private void OnEndLevel()
    {
        if (!_isTrackingLevel) return;
        _timePerLevel[_currentLevelTitle] = Time.time - _levelStartTime;
        _isTrackingLevel = false;
        RebuildLevelInspectorList();
    }

    private void RebuildLevelInspectorList()
    {
        _levelTimings.Clear();
        foreach (var kvp in _timePerLevel)
            _levelTimings.Add(new LevelTimeEntry { name = kvp.Key, levelTitle = kvp.Key, seconds = kvp.Value });
    }

    // ?? Inspector refresh ???????????????????????????????????????????????????

    private void RefreshInspectorView()
    {
        // GameState
        float liveBonus = _isTrackingState ? Time.time - _stateStartTime : 0f;
        float total = GetTotalStateTime() + liveBonus;

        for (int i = 0; i < _stateTimings.Count; i++)
        {
            var entry = _stateTimings[i];
            float t = _timePerState[entry.state] + (_isTrackingState && entry.state == _currentState ? liveBonus : 0f);
            entry.seconds = t;
            entry.percentage = total > 0f ? t / total : 0f;
            _stateTimings[i] = entry;
        }

        // Main menu — do NOT write back here, GetMainMenuTime() adds live bonus on top of stored value
        // Writing _mainMenuTime.seconds = GetMainMenuTime() would compound every frame ? removed

        // Current level (live)
        if (_isTrackingLevel)
        {
            float live = Time.time - _levelStartTime;
            for (int i = 0; i < _levelTimings.Count; i++)
            {
                if (_levelTimings[i].levelTitle != _currentLevelTitle) continue;
                var e = _levelTimings[i];
                e.seconds = live;
                _levelTimings[i] = e;
                break;
            }
        }
    }

    // ?? Public accessors ????????????????????????????????????????????????????

    public float GetTimeInState(GameManager.GameState state) => _timePerState.TryGetValue(state, out float t) ? t : 0f;
    public float GetTimeInLevel(string levelTitle) => _timePerLevel.TryGetValue(levelTitle, out float t) ? t : 0f;
    public float GetMainMenuTime() => _mainMenuTime.seconds + (_isTrackingMenu ? Time.realtimeSinceStartup - _menuStartTime : 0f);
    public float GetTotalStateTime()
    {
        float total = 0f;
        foreach (var t in _timePerState.Values) total += t;
        return total;
    }

    // ?? Debug ???????????????????????????????????????????????????????????????

    [ContextMenu("Log Time Tracking")]
    public void LogTimeTracking()
    {
        if (_isTrackingState) _timePerState[_currentState] += Time.time - _stateStartTime;
        if (_isTrackingLevel) _timePerLevel[_currentLevelTitle] = Time.time - _levelStartTime;

        Debug.Log("=== DEBUG TIME TRACKING ===");
        Debug.Log($"  {"MAIN MENU",-24} {FormatTime(GetMainMenuTime())}");
        Debug.Log("  --- GameStates ---");
        float total = GetTotalStateTime();
        foreach (GameManager.GameState state in Enum.GetValues(typeof(GameManager.GameState)))
        {
            float t = _timePerState[state];
            float pct = total > 0f ? t / total * 100f : 0f;
            Debug.Log($"  {state,-24} {FormatTime(t)}  ({pct:F1}%)");
        }
        Debug.Log("  --- Levels (this session) ---");
        foreach (var kvp in _timePerLevel)
            Debug.Log($"  {kvp.Key,-24} {FormatTime(kvp.Value)}");
        Debug.Log("===========================");

        if (_isTrackingState) _stateStartTime = Time.time;
        if (_isTrackingLevel) _levelStartTime = Time.time;
    }

    [ContextMenu("Reset Time Tracking")]
    public void ResetTimeTracking()
    {
        foreach (GameManager.GameState state in Enum.GetValues(typeof(GameManager.GameState)))
            _timePerState[state] = 0f;

        _timePerLevel.Clear();
        _levelTimings.Clear();
        _mainMenuTime = default;
        _stateStartTime = _levelStartTime = Time.time;
        _menuStartTime = Time.realtimeSinceStartup;
        Debug.Log("Time tracking reset.");
    }

    public static string FormatTime(float seconds)
    {
        int m = (int)(seconds / 60f);
        int s = (int)(seconds % 60f);
        return $"{m}m {s:D2}s";
    }
}