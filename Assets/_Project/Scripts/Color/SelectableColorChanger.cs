using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Selectable))]
public class SelectableColorChanger : MonoBehaviour, IColorChanger
{
    [Header("Color IDs")]
    [SerializeField] private ColorId normalColorId = ColorId.BUTTON_NORMAL;
    [SerializeField] private ColorId highlightedColorId = ColorId.BUTTON_HIGHLIGHTED;
    [SerializeField] private ColorId pressedColorId = ColorId.BUTTON_PRESSED;
    [SerializeField] private ColorId selectedColorId = ColorId.BUTTON_SELECTED;
    [SerializeField] private ColorId disabledColorId = ColorId.BUTTON_DISABLED;

    [Header("Settings")]
    [SerializeField] private bool updateOnStart = true;
    [SerializeField] private bool listenToThemeChanges = true;
    [SerializeField] private float colorMultiplier = 1f;
    [SerializeField] private float fadeDuration = 0.1f;

    private Selectable targetSelectable;

    public ColorId ColorId => normalColorId;

    private void Awake()
    {
        targetSelectable = GetComponent<Selectable>();
    }

    private void Start()
    {
        if (updateOnStart)
        {
            UpdateColor();
        }
    }

    private void OnEnable()
    {
        if (listenToThemeChanges)
        {
            ThemeColorManager.OnThemeChanged += OnThemeChanged;
        }
    }

    private void OnDisable()
    {
        if (listenToThemeChanges)
        {
            ThemeColorManager.OnThemeChanged -= OnThemeChanged;
        }
    }

    public void OnThemeChanged(ColorTheme newTheme)
    {
        UpdateColor();
    }

    public void UpdateColor()
    {
        if (ThemeColorManager.Instance == null)
        {
            Debug.LogWarning("ThemeColorManager instance not found!");
            return;
        }

        if (targetSelectable == null)
        {
            Debug.LogWarning("Button component not found!");
            return;
        }

        ColorBlock colors = targetSelectable.colors;

        colors.normalColor = ThemeColorManager.Instance.GetColor(normalColorId);
        colors.highlightedColor = ThemeColorManager.Instance.GetColor(highlightedColorId);
        colors.pressedColor = ThemeColorManager.Instance.GetColor(pressedColorId);
        colors.selectedColor = ThemeColorManager.Instance.GetColor(selectedColorId);
        colors.disabledColor = ThemeColorManager.Instance.GetColor(disabledColorId);

        colors.colorMultiplier = colorMultiplier;
        colors.fadeDuration = fadeDuration;

        targetSelectable.colors = colors;
    }

#if UNITY_EDITOR
    [ContextMenu("Update Colors Now")]
    private void UpdateColorsEditor()
    {
        if (targetSelectable == null)
        {
            targetSelectable = GetComponent<Selectable>();
        }
        UpdateColor();
    }

#endif
}