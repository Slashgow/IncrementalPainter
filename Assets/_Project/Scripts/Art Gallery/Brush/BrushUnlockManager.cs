using UnityEngine;
/// <summary>
/// Brush unlock manager - inherits from generic UnlockManager
/// </summary>
public class BrushUnlockManager : UnlockManager<BrushData>
{
    private const string SAVE_KEY = "Brushes";

    [Header("Default Unlocks")]
    [SerializeField] private bool unlockDefaultBrushOnStart = true;

    protected override void Start()
    {
        base.Start();

        if (unlockDefaultBrushOnStart)
        {
            UnlockDefaultBrushes();
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        // Subscribe to game events if needed
        // Example: LevelManager.OnEndLevel += CheckAllUnlocks;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        // Unsubscribe from game events
    }

    protected override string GetSaveKey() => SAVE_KEY;

    protected override UnlockableSaveData LoadSaveData()
    {
        BrushSaveData brushSaveData = GameSaveManager.Instance.LoadBrushData();

        UnlockableSaveData saveData = new UnlockableSaveData();
        saveData.unlockedItemIds = new System.Collections.Generic.List<string>(brushSaveData.unlockedBrushIds);

        return saveData;
    }

    protected override void SaveData(UnlockableSaveData saveData)
    {
        BrushSaveData brushSaveData = new BrushSaveData();
        brushSaveData.unlockedBrushIds = new System.Collections.Generic.List<string>(saveData.unlockedItemIds);

        GameSaveManager.Instance.SaveBrushData(brushSaveData);
    }

    private void UnlockDefaultBrushes()
    {
        // Unlock the first brush by default (basic brush)
        if (unlockableItems.Count > 0 && unlockableItems[0].Item != null)
        {
            string firstBrushId = unlockableItems[0].Item.ItemId;
            if (!IsItemUnlocked(firstBrushId))
            {
                UnlockItem(firstBrushId);
            }
        }
    }

    protected override void OnItemLockedCallback(string itemId)
    {
        base.OnItemLockedCallback(itemId);

        // If the currently selected brush is locked, switch to a default unlocked brush
        var brushManager = FindAnyObjectByType<BrushManager>();
        if (brushManager != null && brushManager.CurrentBrush != null)
        {
            if (brushManager.CurrentBrush.ItemId == itemId)
            {
                // Switch to first unlocked brush
                var unlockedBrushes = GetUnlockedItems();
                if (unlockedBrushes.Count > 0)
                {
                    brushManager.SetBrush(unlockedBrushes[0]);
                }
            }
        }
    }

    public bool CanUseBrush(string brushId) => IsItemUnlocked(brushId);
    public bool CanUseBrush(BrushData brush) => brush != null && CanUseBrush(brush.ItemId);
}
