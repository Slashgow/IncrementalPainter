using System;
using PaintCore;
using UnityEngine;

public class Level : MonoBehaviour, ISavable, ILoadable<LevelSaveData>
{
    [SerializeField] private Collider2D frameCollider;
    [SerializeField] private SpriteRenderer frameRenderer;
    [SerializeField] private SpriteRenderer drawingRenderer;
    [SerializeField] private CwChangeCounter colorChangeCounter;

    private LevelData levelData;
    private LevelSaveData saveData;
    public SpriteRenderer FrameRenderer => frameRenderer;
    public Collider2D FrameCollider => frameCollider;
    public CwChangeCounter ColorChangeCounter => colorChangeCounter;
    public bool IsDoneCondition => colorChangeCounter.Ratio >= levelData.PercentCompletionCondition;

    public event Action OnEndLevel;
    public void Initialize(LevelData levelData)
    {
        this.levelData = levelData;
        saveData = Load();

        drawingRenderer.sprite = levelData.LevelDrawing;

        colorChangeCounter.OnUpdated += ColorChangeCounter_OnUpdated;
        GameManager.OnStartGameState += GameManager_OnStartGameState;
    }

    private void OnDestroy()
    {
        colorChangeCounter.OnUpdated -= ColorChangeCounter_OnUpdated;
        if(GameManager.HasInstance)
            GameManager.OnStartGameState -= GameManager_OnStartGameState;
    }

    private void ColorChangeCounter_OnUpdated()
    {
        if (!saveData.isDone && IsDoneCondition)
        {
            Save();
            OnEndLevel?.Invoke();
        }  
    }
    private void GameManager_OnStartGameState(GameManager.GameState gameState)
    {
        if (gameState != GameManager.GameState.DAY_SUMMARY)
            return;

        Save();
    }

    public LevelSaveData Load() => GameSaveManager.Instance.LoadLevelData(levelData.LevelAuthor, levelData.LevelTitle);
    public void Save()
    {
        GameSaveManager.Instance.SaveLevelData(IsDoneCondition, colorChangeCounter.Ratio,
        levelData.LevelAuthor, levelData.LevelTitle);
    }
}
