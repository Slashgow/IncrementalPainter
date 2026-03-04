using System;
using UnityEngine;

public class TamponUnlockPopupSpawner : PopupSpawner<TamponData, TamponUnlockManager>
{
    protected override void ShowPopup(string id, Action onClosed)
    {
        TamponData unlockedTampon = TamponUnlockManager.Instance.GetItemById(id);
        if (unlockedTampon == null)
        {
            onClosed?.Invoke();
            return;
        }

        Transform parent = popupParent != null ? popupParent.transform : transform;
        UnlockTamponPopup popup = (UnlockTamponPopup)Instantiate(popupPrefab, parent);
        popup.OnClosed = onClosed;
        popup.Initialize(unlockedTampon, popupParent);
    }
}
