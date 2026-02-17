using System;
using System.Collections.Generic;
using inkolorgames;
using PaintCore;
using PaintIn2D;
using UnityEngine;
public class BrushManager : PersistentMonoSingleton<BrushManager>
{
    [Header("References")]
    [SerializeField] private BrushUnlockManager unlockManager;
    [SerializeField] private CwPaintDecal2D paintDecal2D;

    [Header("Default Brush")]
    [SerializeField] private BrushData defaultBrush;
    [SerializeField, Range(0f, 100f)] private float defaultSize;
    [SerializeField, Range(0f, 100f)] private float minSize;
    [SerializeField, Range(0f, 100f)] private float maxSize;

    public static event Action<BrushData> OnBrushChanged;
    public static event Action<Texture> OnBrushTextureChanged;
    public static event Action<float> OnBrushSizeChanged;
    public static event Action<float> OnBrushOpacityChanged;

    private float currentSize = 10f;
    private float currentOpacity = 1f;
    private BrushData currentBrush;
    public BrushData CurrentBrush => currentBrush;
    public BrushData DefaultBrush => defaultBrush;
    public float MinSize => minSize;
    public float MaxSize => maxSize;
    public float CurrentSize => currentSize;
    public float CurrentOpacity => currentOpacity;

    public void EnableBrushPainting() => paintDecal2D.gameObject.SetActive(true);
    public void DisableBrushPainting() => paintDecal2D.gameObject.SetActive(false);

    protected override void Awake()
    {
        base.Awake();
        SetSize(defaultSize, false);
        ArtGalleryColorManager.OnColorChanged += SetColor;
        ArtGalleryColorManager.OnApplyColorRandomModifier += SetColorModifier;
        ArtGalleryColorManager.OnRemoveColorRandomModifier += RemoveColorModifier;
        ArtGaleryInput.OnBrushSizeChanged += ChangeSize;
        ArtGaleryInput.OnBrushOpacityChanged += ChangeOpacity;
    }

    private void OnDestroy()
    {
        ArtGalleryColorManager.OnColorChanged -= SetColor;
        ArtGalleryColorManager.OnApplyColorRandomModifier -= SetColorModifier;
        ArtGalleryColorManager.OnRemoveColorRandomModifier -= RemoveColorModifier;
        ArtGaleryInput.OnBrushSizeChanged -= ChangeSize;
        ArtGaleryInput.OnBrushOpacityChanged -= ChangeOpacity;
    }

    private void Start()
    {
        LoadBrushData();

        if (currentBrush == null)
        {
            SetDefaultBrush();
        }
    }

    private void LoadBrushData()
    {
        BrushSaveData saveData = GameSaveManager.Instance.LoadBrushData();

        // Unlock default brush if needed
        if (defaultBrush != null && !unlockManager.IsItemUnlocked(defaultBrush.ItemId))
        {
            unlockManager.UnlockItem(defaultBrush.ItemId);
        }

        // Load saved brush
        if (!string.IsNullOrEmpty(saveData.currentBrushId))
        {
            BrushData savedBrush = unlockManager.GetItemById(saveData.currentBrushId);
            if (savedBrush != null && unlockManager.IsItemUnlocked(savedBrush.ItemId))
            {
                SetBrush(savedBrush, false);
            }
        }
    }

    private void SetDefaultBrush()
    {
        if (defaultBrush != null)
        {
            SetBrush(defaultBrush);
        }
        else
        {
            // Fallback to first unlocked brush
            var unlockedBrushes = unlockManager.GetUnlockedItems();
            if (unlockedBrushes.Count > 0)
            {
                SetBrush(unlockedBrushes[0]);
            }
        }
    }

    public void SetBrush(BrushData brush, bool saveToFile = true)
    {
        if (brush == null)
        {
            Debug.LogError("Cannot set null brush!");
            return;
        }

        if (!unlockManager.IsItemUnlocked(brush.ItemId))
        {
            Debug.LogWarning($"Brush '{brush.BrushName}' is locked!");
            return;
        }

        currentBrush = brush;
        SetTexture(brush.BrushTexture, true);


        if (saveToFile)
        {
            BrushSaveData saveData = GameSaveManager.Instance.LoadBrushData();
            saveData.currentBrushId = brush.ItemId;
            GameSaveManager.Instance.SaveBrushData(saveData);
        }

        OnBrushChanged?.Invoke(currentBrush);
        Debug.Log($"Brush changed to: {brush.BrushName}");
    }

    public void SetBrushById(string brushId)
    {
        BrushData brush = unlockManager.GetItemById(brushId);
        if (brush != null)
        {
            SetBrush(brush);
        }
        else
        {
            Debug.LogError($"Brush with ID '{brushId}' not found!");
        }
    }

    public void SetTexture(Texture texture, bool notifyListeners = true)
    {
        paintDecal2D.Texture = texture;

        if (notifyListeners)
            OnBrushTextureChanged?.Invoke(texture);
    }

    public void ChangeSize(float delta)
    {
        if(ToolStateMachine.CurrentState.ToolType != ToolType.Brush)
            return;

        SetSize(currentSize + delta, true);
    }

    public void SetSize(float size, bool notifyListeners = false)
    {
        currentSize = Mathf.Clamp(size, minSize, maxSize);
        paintDecal2D.Scale = Vector3.one * currentSize;

        if (notifyListeners)
            OnBrushSizeChanged?.Invoke(currentSize);
    }

    public void SetColor(Color color)
    {
        paintDecal2D.Color = color;
    }

    public void SetColorModifier(CwModifyColorRandom colorModifier = null)
    {
        paintDecal2D.Modifiers.Instances.Add(colorModifier);
    }

    public void RemoveColorModifier(CwModifyColorRandom colorModifier)
    {
        paintDecal2D.Modifiers.Instances.Remove(colorModifier);
    }

    public void ChangeOpacity(float delta)
    {
        if(ToolStateMachine.CurrentState.ToolType != ToolType.Brush)
            return;

        SetOpacity(currentOpacity + delta, true);
    }

    public void SetOpacity(float opacity, bool notifyListeners = false)
    {
        currentOpacity = Mathf.Clamp01(opacity);
        paintDecal2D.Opacity = opacity;

        if (notifyListeners)
            OnBrushOpacityChanged?.Invoke(currentOpacity);
    }

    public bool IsBrushUnlocked(string brushId) => unlockManager.IsItemUnlocked(brushId);
    public bool IsBrushUnlocked(BrushData brush) => brush != null && IsBrushUnlocked(brush.ItemId);

    public List<BrushData> GetUnlockedBrushes() => unlockManager.GetUnlockedItems();
    public List<BrushData> GetLockedBrushes() => unlockManager.GetLockedItems();
    public List<Unlockable<BrushData>> GetAllUnlockableBrushes() => unlockManager.GetAllUnlockables();

    public string GetUnlockDescription(BrushData brush)
    {
        return unlockManager.GetUnlockDescription(brush);
    }

    public Sprite GetCurrentBrushSprite() => currentBrush?.BrushSprite;


#if UNITY_EDITOR
    [ContextMenu("Reset to Default Brush")]
    private void ResetToDefaultBrush()
    {
        SetDefaultBrush();
    }
#endif
}