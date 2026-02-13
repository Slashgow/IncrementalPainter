using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIColorLoader : MonoBehaviour
{
    [SerializeField] private Toggle toggleColorVariation;
    [SerializeField] private Transform parent;
    [SerializeField] private UIPalettePreviewer palettePreviewer;

    private void OnEnable() => toggleColorVariation.onValueChanged.AddListener(OnToggleValueChanged);
    private void OnDisable() => toggleColorVariation.onValueChanged.RemoveListener(OnToggleValueChanged);
    private void OnToggleValueChanged(bool enable) => ArtGalleryColorManager.Instance.ToggleColorModifier(enable);

    public void Start()
    {
        toggleColorVariation.isOn = false;

        InitializeColorUI();
    }

    private void InitializeColorUI()
    {
        parent.DestroyAllChildren();

        List<Unlockable<ColorTheme>> unlockedColorThemes = ThemeColorManager.Instance.GetUnlockedThemes();

        foreach (Unlockable<ColorTheme> unlockedColorTheme in unlockedColorThemes)
        {
            UIPalettePreviewer uiPalettePreviewerInstance = Instantiate(palettePreviewer, parent);
            uiPalettePreviewerInstance.Initialize(unlockedColorTheme.Item);
        }
    }
}
