using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public class UnlockThemePopup : PopUp
{
    [SerializeField] private TextMeshProUGUI themeNameText;
    [SerializeField] private Image previewImage;
    [SerializeField] private LocalizedString unlockLocalizedString;
    [SerializeField] private Transform paletteParent;
    [SerializeField] private PreviewColorImage previewColorImagePrefab;

    private ColorTheme unlockedTheme;
    private Canvas parentCanvas;

    public void Initialize(ColorTheme theme, Canvas parent)
    {
        unlockedTheme = theme;
        parentCanvas = parent;

#if !UNITY_WEBGL
        themeNameText.text = $"{theme.ThemeName} {unlockLocalizedString.GetLocalizedString()}";
#else
        unlockLocalizedString.GetLocalizedStringAsync().Completed += (handle) =>
        {
            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                themeNameText.text = $"{theme.ThemeName} {handle.Result}";
        };
#endif

        previewImage.color = theme.GetColor(ColorId.PRIMARY);
        GenerateColorPalette(theme);
        ThemeColorManager.Instance.SetTheme(unlockedTheme);

        parentCanvas.gameObject.SetActive(true);
        gameObject.SetActive(true);

        if (GameManager.HasInstance)
            GameManager.Instance.Pause();
    }

    protected override void OnClickDoActionButton()
    {
        ThemeColorManager.Instance.SetTheme(unlockedTheme);
        Dismiss();
    }

    protected override void OnClickCancelButton()
    {
        Dismiss();
    }

    private void Dismiss()
    {
        gameObject.SetActive(false);

        // Only hide the canvas if no more sibling popups are active
        if (parentCanvas != null && !HasActivePopupSibling())
            parentCanvas.gameObject.SetActive(false);

        if (GameManager.HasInstance)
            GameManager.Instance.Resume();

        Close(); // notifies the spawner queue
    }

    private bool HasActivePopupSibling()
    {
        foreach (Transform child in parentCanvas.transform)
            if (child.gameObject != gameObject && child.gameObject.activeSelf && child.GetComponent<PopUp>() != null)
                return true;
        return false;
    }

    private void GenerateColorPalette(ColorTheme theme)
    {
        if (paletteParent == null || previewColorImagePrefab == null)
            return;

        paletteParent.DestroyAllChildren();

        foreach (ColorId colorId in Enum.GetValues(typeof(ColorId)))
        {
            Color color = theme.GetColor(colorId);
            PreviewColorImage preview = Instantiate(previewColorImagePrefab, paletteParent);
            preview.Initialize(color);
        }
    }
}