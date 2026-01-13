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

        themeNameText.text = $"{theme.ThemeName} {unlockLocalizedString.GetLocalizedString()}";
        previewImage.color = theme.GetColor(ColorId.PRIMARY);

        GenerateColorPalette(theme);

        if (unlockedTheme != null)
        {
            ThemeColorManager.Instance.SetTheme(unlockedTheme);
            Debug.Log($"Switched to theme: {unlockedTheme.ThemeName}");
        }

        parentCanvas.gameObject.SetActive(true);
        gameObject.SetActive(true);
        GameManager.Instance.Pause();
    }

    protected override void OnClickDoActionButton()
    {
        base.OnClickDoActionButton();

        if (unlockedTheme != null)
        {
            ThemeColorManager.Instance.SetTheme(unlockedTheme);
            Debug.Log($"Switched to theme: {unlockedTheme.ThemeName}");
        }

        gameObject.SetActive(false);
        parentCanvas.gameObject.SetActive(false);
        GameManager.Instance.Resume();
    }

    protected override void OnClickCancelButton()
    {
        base.OnClickCancelButton();
        parentCanvas.gameObject.SetActive(false);
        GameManager.Instance.Resume();
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
