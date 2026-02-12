using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Manages brush selection and properties, integrates with unlock system
/// </summary>
public class BrushManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BrushUnlockManager unlockManager;

    [Header("Current Brush")]
    [SerializeField] private BrushData currentBrush;
    [SerializeField] private BrushData defaultBrush;

    [Header("Current Properties")]
    [SerializeField] private float currentSize = 10f;
    [SerializeField] private Color currentColor = Color.black;
    [SerializeField] private float currentOpacity = 1f;

    // Events
    public static event Action<BrushData> OnBrushChanged;
    public static event Action<float> OnBrushSizeChanged;
    public static event Action<Color> OnBrushColorChanged;
    public static event Action<float> OnBrushOpacityChanged;

    // Properties
    public BrushData CurrentBrush => currentBrush;
    public BrushData DefaultBrush => defaultBrush;
    public float CurrentSize => currentSize;
    public Color CurrentColor => currentColor;
    public float CurrentOpacity => currentOpacity;

    private void Awake()
    {
        if (unlockManager == null)
        {
            unlockManager = FindObjectOfType<BrushUnlockManager>();
        }
    }

    private void Start()
    {
        LoadBrushData();

        // Ensure we have a brush selected
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

        // Check if brush is unlocked
        if (!unlockManager.IsItemUnlocked(brush.ItemId))
        {
            Debug.LogWarning($"Brush '{brush.BrushName}' is locked!");
            return;
        }

        currentBrush = brush;

        // Reset properties to brush defaults
        SetSize(brush.DefaultSize, false);
        SetOpacity(brush.DefaultOpacity, false);

        // Save to file
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

    public void SetSize(float size, bool notifyListeners = true)
    {
        if (currentBrush != null)
        {
            currentSize = Mathf.Clamp(size, currentBrush.MinSize, currentBrush.MaxSize);
        }
        else
        {
            currentSize = Mathf.Max(1f, size);
        }

        if (notifyListeners)
        {
            OnBrushSizeChanged?.Invoke(currentSize);
        }
    }

    public void SetColor(Color color, bool notifyListeners = true)
    {
        if (currentBrush != null && !currentBrush.SupportsColorChange)
        {
            Debug.LogWarning($"Brush '{currentBrush.BrushName}' does not support color changes!");
            return;
        }

        currentColor = color;

        if (notifyListeners)
        {
            OnBrushColorChanged?.Invoke(currentColor);
        }
    }

    public void SetOpacity(float opacity, bool notifyListeners = true)
    {
        currentOpacity = Mathf.Clamp01(opacity);

        if (notifyListeners)
        {
            OnBrushOpacityChanged?.Invoke(currentOpacity);
        }
    }

    // Query methods
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

    public Color GetCurrentColorWithOpacity()
    {
        Color color = currentColor;
        color.a = currentOpacity;
        return color;
    }

#if UNITY_EDITOR
    [ContextMenu("Reset to Default Brush")]
    private void ResetToDefaultBrush()
    {
        SetDefaultBrush();
    }
#endif
}