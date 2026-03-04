using UnityEngine;

public class TamponUnlockPopupSpawner : PopupSpawner<TamponData, TamponUnlockManager>
{
    protected override void OnItemUnlocked(string id)
    {
        TamponData unlockedTampon = TamponUnlockManager.Instance.GetItemById(id);
        if (unlockedTampon != null)
        {
            Debug.Log($"Unlocked Tampon: {unlockedTampon.ItemName}");
        }
        Transform parent = popupParent != null ? popupParent.transform : transform;
        UnlockTamponPopup popup = (UnlockTamponPopup)Instantiate(popupPrefab, parent);

        popup.Initialize(unlockedTampon, popupParent);
    }
}
