using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIPalettePreviewer : MonoBehaviour
{
    [SerializeField] private Transform colorsContainer;
    [SerializeField] private TextMeshProUGUI themeNameText;
    [SerializeField] private UIColorSelector colorImagePrefab;

    private ColorTheme colorTheme;

    private List<UIColorSelector> colorSelectors = new List<UIColorSelector>();

    public void Initialize(ColorTheme theme)
    {
        colorTheme = theme;
        themeNameText.text = theme.ThemeName;
        UpdateColorPreviews();
    }

    private void UpdateColorPreviews()
    {
        colorsContainer.DestroyAllChildren();
        foreach (ColorTheme.ColorEntry entry in colorTheme.Colors)
        {
            UIColorSelector uiColorSelectorInstance = Instantiate(colorImagePrefab, colorsContainer);
            uiColorSelectorInstance.Initialize(entry.color);
            colorSelectors.Add(uiColorSelectorInstance);
            uiColorSelectorInstance.OnSelectEvent += OnColorSelected;
        }
    }

    private void OnColorSelected(Color color)
    {
        ArtGalleryColorManager.Instance.SetCurrentColor(color);
    }

    private void OnDestroy()
    {
        foreach (UIColorSelector colorSelector in colorSelectors)
        {
            colorSelector.OnSelectEvent -= OnColorSelected;
        }
    }
}
