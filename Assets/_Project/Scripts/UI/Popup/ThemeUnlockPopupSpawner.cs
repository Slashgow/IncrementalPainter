using UnityEngine;

public class ThemeUnlockPopupSpawner : PopupSpawner<ColorTheme, ColorThemeUnlockManager>
{
    protected override void OnItemUnlocked(string id)
    {
        ColorTheme unlockedTheme = ThemeColorManager.Instance.GetThemeById(id);

        if (unlockedTheme == null)
        {
            Debug.LogWarning($"Could not find theme with ID: {id}");
            return;
        }

        Transform parent = popupParent != null ? popupParent.transform : transform;
        UnlockThemePopup popup = (UnlockThemePopup)Instantiate(popupPrefab, parent);

        popup.Initialize(unlockedTheme, popupParent);
    }
}
