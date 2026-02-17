using System;
using PaintCore;
using PaintIn2D;
using UnityEngine;

public class Level : MonoBehaviour, ISavable, ILoadable<LevelSaveData>
{
    [SerializeField] private Collider2D frameCollider;
    [SerializeField] private SpriteRenderer frameRenderer;
    [SerializeField] private SpriteRenderer drawingRenderer;
    [SerializeField] private CwChangeCounter colorChangeCounter;
    [SerializeField] private Transform bossSpawnTransform;

    [Header("Art Gallery Settings")]
    [SerializeField] private SpriteRenderer backgroundSpriteRenderer;
    [SerializeField] private CwPaintableSpriteTexture paintableSprite;
    [SerializeField] private Draggable draggable;
    [SerializeField] private BoxCollider2D draggableBoxCollider2D;

    private LevelData levelData;
    private LevelSaveData saveData;
    private bool isArtGallery;

    public LevelData LevelData => levelData;
    public SpriteRenderer FrameRenderer => frameRenderer;
    public Collider2D FrameCollider => frameCollider;
    public CwChangeCounter ColorChangeCounter => colorChangeCounter;
    public Transform BossSpawnTransform => bossSpawnTransform;
    public bool IsDoneCondition => colorChangeCounter.Ratio >= levelData.PercentCompletionCondition;

    public event Action OnEndLevel;
    public event Action OnMidLevel;

    private bool isMidConditionRaised = false;
    public bool IsMidCondition => colorChangeCounter.Ratio >= 0.5f;

    private void Start()
    {
        colorChangeCounter.OnUpdated += ColorChangeCounter_OnUpdated;
        GameManager.OnStartGameState += GameManager_OnStartGameState;
    }

    private void OnDisable()
    {
        colorChangeCounter.OnUpdated -= ColorChangeCounter_OnUpdated;
        GameManager.OnStartGameState -= GameManager_OnStartGameState;
    }

    public void Initialize(LevelData levelData, bool isArtGallery)
    {
        this.isArtGallery = isArtGallery;
        this.levelData = levelData;
        drawingRenderer.sprite = levelData.LevelDrawing;
    
        saveData = Load();
   
 
        if (isArtGallery)
        {
            draggable.enabled = true;
            colorChangeCounter.enabled = false;
            draggableBoxCollider2D.enabled = true;
            paintableSprite.enabled = false;
            this.gameObject.layer = LayerMask.NameToLayer("Draggable");

            if (backgroundSpriteRenderer != null)
                backgroundSpriteRenderer.gameObject.SetActive(false);
        }
        else
        {
            draggable.enabled = false;
            draggableBoxCollider2D.enabled = false;
        }
    }

    private void ColorChangeCounter_OnUpdated()
    {
        if(isArtGallery)
            return;

        Debug.Log($"Color change ratio: {colorChangeCounter.Ratio:P2}");

        if (!isMidConditionRaised && IsMidCondition)
        {
            OnMidLevel?.Invoke();
        }

        if (!saveData.isDone && IsDoneCondition)
        {
            Save();
            OnEndLevel?.Invoke();
        }  
    }
    private void GameManager_OnStartGameState(GameManager.GameState gameState)
    {
        if(isArtGallery)
            return;

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

    public ArtGalleryPaintingSaveData GetArtGalleryTransformData()
    {
        return new ArtGalleryPaintingSaveData(this.transform, this.levelData.LevelAuthor, this.levelData.LevelTitle);
    }

    public void LoadArtGalleryPaintingSaveData(ArtGalleryPaintingSaveData transformData)
    {
        if (transformData != null)
            transformData.ApplyTo(this.transform);
    }
}
