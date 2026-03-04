using System;
using System.Collections.Generic;
using inkolorgames;
using UnityEngine;

public abstract class PopupSpawner<TItem, TManager> : MonoBehaviour
    where TItem : ScriptableObject, IUnlockableItem
    where TManager : MonoSingleton<TManager>
{
    [SerializeField] private UnlockManager<TItem, TManager> unlockManager;
    [SerializeField] protected PopUp popupPrefab;
    [SerializeField] protected Canvas popupParent;

    private readonly Queue<string> pendingIds = new();
    private bool isShowingPopup = false;


    private void Awake() => popupParent.gameObject.SetActive(false);
    private void OnEnable() => unlockManager.OnItemUnlocked += OnItemUnlocked;
    private void OnDisable() => unlockManager.OnItemUnlocked -= OnItemUnlocked;
    private void OnItemUnlocked(string id)
    {
        pendingIds.Enqueue(id);
        TryShowNext();
    }

    private void TryShowNext()
    {
        if (isShowingPopup || pendingIds.Count == 0)
            return;

        isShowingPopup = true;
        string id = pendingIds.Dequeue();
        ShowPopup(id, OnPopupClosed);
    }

    private void OnPopupClosed()
    {
        isShowingPopup = false;
        TryShowNext();
    }

    protected abstract void ShowPopup(string id, Action onClosed);
}
