
using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Generic wrapper that makes any ScriptableObject unlockable
/// </summary>
/// <typeparam name="T">Type of item (must be ScriptableObject and IUnlockableItem)</typeparam>
[Serializable]
public class Unlockable<T> : IUnlockable where T : ScriptableObject, IUnlockableItem
{
    [SerializeField] private T item;
    [SerializeField] private List<UnlockCondition> unlockConditions = new List<UnlockCondition>();

    public T Item => item;
    public List<UnlockCondition> UnlockConditions => unlockConditions;

    private IUnlockManager<T> unlockManager;

    public bool IsUnlocked => CheckUnlockCondition();

    public void Initialize(IUnlockManager<T> manager)
    {
        unlockManager = manager;
    }

    public bool CheckUnlockCondition()
    {
        if (item == null)
        {
            Debug.LogWarning("Item is null in Unlockable!");
            return false;
        }

        // If no conditions, it's unlocked by default
        if (unlockConditions == null || unlockConditions.Count == 0)
            return true;

        // Check if already unlocked in save data
        if (unlockManager != null && unlockManager.IsItemUnlocked(item.ItemId))
            return true;

        // Check if all conditions are met
        bool allMet = true;
        foreach (var condition in unlockConditions)
        {
            if (!condition.IsMet())
            {
                allMet = false;
                break;
            }
        }

        // Auto-unlock if all conditions met
        if (allMet && unlockManager != null)
        {
            Unlock();
        }

        return allMet;
    }

    public void Unlock()
    {
        if (item == null)
        {
            Debug.LogWarning("Cannot unlock - Item is null!");
            return;
        }

        if (unlockManager == null)
        {
            Debug.LogWarning($"Cannot unlock - No unlock manager set for {item.ItemName}!");
            return;
        }

        if (unlockManager.IsItemUnlocked(item.ItemId))
            return;

        unlockManager.UnlockItem(item.ItemId);
        Debug.Log($"{item.ItemType} '{item.ItemName}' unlocked!");
    }

    public void Lock()
    {
        if (item == null || unlockManager == null)
            return;

        unlockManager.LockItem(item.ItemId);
    }

    public string GetUnlockDescription()
    {
        if (unlockConditions == null || unlockConditions.Count == 0)
            return "Already unlocked";

        if (IsUnlocked)
            return "Unlocked";

        string description = "Unlock conditions:\n";
        for (int i = 0; i < unlockConditions.Count; i++)
        {
            description += $"- {unlockConditions[i].GetDescription()}";
            if (i < unlockConditions.Count - 1)
                description += "\n";
        }

        return description;
    }
}
