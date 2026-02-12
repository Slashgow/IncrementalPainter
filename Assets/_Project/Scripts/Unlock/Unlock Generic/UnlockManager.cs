using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Generic base class for unlock managers. Inherit from this to create managers for specific item types.
/// </summary>
/// <typeparam name="T">Type of unlockable item (must be ScriptableObject and IUnlockableItem)</typeparam>
public abstract class UnlockManager<T> : MonoBehaviour, IUnlockManager<T>
    where T : ScriptableObject, IUnlockableItem
{
    [SerializeField] protected List<Unlockable<T>> unlockableItems = new List<Unlockable<T>>();
    [SerializeField] protected bool checkUnlocksOnStart = true;
    [SerializeField] protected bool autoUnlockItems = true;

    // Events
    public event Action<string> OnItemUnlocked;
    public event Action<string> OnItemLocked;

    // Abstract methods that must be implemented by derived classes
    protected abstract string GetSaveKey();
    protected abstract UnlockableSaveData LoadSaveData();
    protected abstract void SaveData(UnlockableSaveData saveData);

    protected virtual void Awake()
    {
        InitializeUnlockables();
    }

    protected virtual void OnEnable()
    {
        // Override in derived class to subscribe to game events
        // Example: LevelManager.OnEndLevel += CheckAllUnlocks;
    }

    protected virtual void OnDisable()
    {
        // Override in derived class to unsubscribe from game events
        // Example: LevelManager.OnEndLevel -= CheckAllUnlocks;
    }

    protected virtual void Start()
    {
        if (checkUnlocksOnStart)
        {
            CheckAllUnlocks();
        }
    }

    private void InitializeUnlockables()
    {
        foreach (var unlockable in unlockableItems)
        {
            unlockable.Initialize(this);
        }
    }

    public virtual void CheckAllUnlocks()
    {
        if (!autoUnlockItems)
            return;

        foreach (var unlockable in unlockableItems)
        {
            unlockable.CheckUnlockCondition();
        }
    }

    public void CheckItemUnlock(string itemId)
    {
        Unlockable<T> unlockable = GetUnlockable(itemId);
        if (unlockable != null)
        {
            unlockable.CheckUnlockCondition();
        }
    }

    public void CheckItemUnlock(T item)
    {
        if (item != null && !string.IsNullOrEmpty(item.ItemId))
        {
            CheckItemUnlock(item.ItemId);
        }
    }

    public Unlockable<T> GetUnlockable(string itemId)
    {
        return unlockableItems.Find(u => u.Item != null && u.Item.ItemId == itemId);
    }

    public Unlockable<T> GetUnlockable(T item)
    {
        if (item == null)
            return null;

        return GetUnlockable(item.ItemId);
    }

    public List<Unlockable<T>> GetAllUnlockables()
    {
        return new List<Unlockable<T>>(unlockableItems);
    }

    public List<Unlockable<T>> GetLockedUnlockables()
    {
        List<Unlockable<T>> locked = new List<Unlockable<T>>();

        foreach (var unlockable in unlockableItems)
        {
            if (!unlockable.IsUnlocked)
            {
                locked.Add(unlockable);
            }
        }

        return locked;
    }

    public List<Unlockable<T>> GetUnlockedUnlockables()
    {
        List<Unlockable<T>> unlocked = new List<Unlockable<T>>();

        foreach (var unlockable in unlockableItems)
        {
            if (unlockable.IsUnlocked)
            {
                unlocked.Add(unlockable);
            }
        }

        return unlocked;
    }

    public bool IsItemUnlockable(string itemId)
    {
        Unlockable<T> unlockable = GetUnlockable(itemId);
        return unlockable != null;
    }

    public bool IsItemUnlockable(T item)
    {
        if (item == null)
            return false;

        return IsItemUnlockable(item.ItemId);
    }

    public string GetUnlockDescription(string itemId)
    {
        Unlockable<T> unlockable = GetUnlockable(itemId);

        if (unlockable != null)
        {
            return unlockable.GetUnlockDescription();
        }

        return "Item not found";
    }

    public string GetUnlockDescription(T item)
    {
        if (item == null)
            return "Item is null";

        return GetUnlockDescription(item.ItemId);
    }

    // IUnlockManager implementation
    public virtual bool IsItemUnlocked(string itemId)
    {
        UnlockableSaveData saveData = LoadSaveData();
        return saveData.IsItemUnlocked(itemId);
    }

    public virtual void UnlockItem(string itemId)
    {
        UnlockableSaveData saveData = LoadSaveData();

        if (!saveData.IsItemUnlocked(itemId))
        {
            saveData.UnlockItem(itemId);
            SaveData(saveData);
            OnItemUnlocked?.Invoke(itemId);
            Debug.Log($"Item '{itemId}' unlocked!");
        }
    }

    public virtual void LockItem(string itemId)
    {
        UnlockableSaveData saveData = LoadSaveData();

        if (saveData.IsItemUnlocked(itemId))
        {
            saveData.LockItem(itemId);
            SaveData(saveData);
            OnItemLocked?.Invoke(itemId);
            OnItemLockedCallback(itemId);
            Debug.Log($"Item '{itemId}' locked!");
        }
    }

    protected virtual void OnItemLockedCallback(string itemId)
    {
        // Override in derived class for custom behavior when item is locked
    }

    public void UnlockItem(T item)
    {
        if (item != null && !string.IsNullOrEmpty(item.ItemId))
        {
            UnlockItem(item.ItemId);
        }
    }

    public void LockItem(T item)
    {
        if (item != null && !string.IsNullOrEmpty(item.ItemId))
        {
            LockItem(item.ItemId);
        }
    }

    public T GetItemById(string itemId)
    {
        Unlockable<T> unlockable = GetUnlockable(itemId);
        return unlockable?.Item;
    }

    public List<T> GetUnlockedItems()
    {
        UnlockableSaveData saveData = LoadSaveData();
        List<T> unlockedItems = new List<T>();

        foreach (var unlockable in unlockableItems)
        {
            if (unlockable.Item != null && saveData.IsItemUnlocked(unlockable.Item.ItemId))
            {
                unlockedItems.Add(unlockable.Item);
            }
        }

        return unlockedItems;
    }

    public List<T> GetLockedItems()
    {
        UnlockableSaveData saveData = LoadSaveData();
        List<T> lockedItems = new List<T>();

        foreach (var unlockable in unlockableItems)
        {
            if (unlockable.Item != null && !saveData.IsItemUnlocked(unlockable.Item.ItemId))
            {
                lockedItems.Add(unlockable.Item);
            }
        }

        return lockedItems;
    }

    public void ForceUnlock(string itemId)
    {
        Unlockable<T> unlockable = GetUnlockable(itemId);
        if (unlockable != null)
        {
            unlockable.Unlock();
        }
    }

    public void ForceUnlock(T item)
    {
        if (item != null)
        {
            ForceUnlock(item.ItemId);
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Check All Unlocks Now")]
    private void CheckAllUnlocksEditor()
    {
        CheckAllUnlocks();
    }

    [ContextMenu("Force Unlock All Items")]
    private void ForceUnlockAllEditor()
    {
        foreach (var unlockable in unlockableItems)
        {
            unlockable.Unlock();
        }
    }

    [ContextMenu("Lock All Items")]
    private void LockAllEditor()
    {
        foreach (var unlockable in unlockableItems)
        {
            unlockable.Lock();
        }
    }
#endif
}