using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightColorChanger : MonoBehaviour, IColorChanger
{
    [SerializeField] private Light2D targetLight;
    [SerializeField] private ColorId colorId;
    [SerializeField] private bool updateOnStart = true;
    private void Start()
    {
        if (updateOnStart)
            UpdateColor();
    }

    private void OnEnable() => ThemeColorManager.OnThemeChanged += OnThemeChanged;
    private void OnDisable() => ThemeColorManager.OnThemeChanged -= OnThemeChanged;
    public void OnThemeChanged(ColorTheme newTheme) => UpdateColor();
    public void UpdateColor() => targetLight.color = ThemeColorManager.Instance.GetColor(colorId);
}
