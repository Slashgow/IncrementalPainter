using System;
using UnityEngine;

public class ThemeUnlockPopupSpawner : PopupSpawner<ColorTheme, ColorThemeUnlockManager>
{
    protected override void ShowPopup(string id, Action onClosed)
    {
        ColorTheme unlockedTheme = ThemeColorManager.Instance.GetThemeById(id);
        if (unlockedTheme == null)
        {
            Debug.LogWarning($"Could not find theme with ID: {id}");
            onClosed?.Invoke();
            return;
        }

        Transform parent = popupParent != null ? popupParent.transform : transform;
        UnlockThemePopup popup = (UnlockThemePopup)Instantiate(popupPrefab, parent);
        popup.OnClosed = onClosed;
        popup.Initialize(unlockedTheme, popupParent);
    }
}
