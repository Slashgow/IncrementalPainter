using UnityEngine;

public class ThemeUnlockPopupSpawner : MonoBehaviour
{
    [SerializeField] private UnlockThemePopup unlockThemePopupPrefab;
    [SerializeField] private Canvas popupParent;

    private void Awake() => popupParent.gameObject.SetActive(false);
    private void OnEnable() => ColorThemeUnlockManager.OnThemeUnlocked += OnThemeUnlocked;
    private void OnDisable() => ColorThemeUnlockManager.OnThemeUnlocked -= OnThemeUnlocked;

    private void OnThemeUnlocked(string themeId)
    {
        ColorTheme unlockedTheme = ThemeColorManager.Instance.GetThemeById(themeId);

        if (unlockedTheme == null)
        {
            Debug.LogWarning($"Could not find theme with ID: {themeId}");
            return;
        }

        Transform parent = popupParent != null ? popupParent.transform : transform;
        UnlockThemePopup popup = Instantiate(unlockThemePopupPrefab, parent);

        popup.Initialize(unlockedTheme, popupParent);
    }
}
