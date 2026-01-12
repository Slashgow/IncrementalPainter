using TabUI;
using UnityEngine;

public class TabGroupColorChanger : TabGroup, IColorChanger
{
    [SerializeField] private ColorId tabIdleID = ColorId.BUTTON_NORMAL;
    [SerializeField] private ColorId tabHoverID = ColorId.BUTTON_HIGHLIGHTED;
    [SerializeField] private ColorId tabActiveID = ColorId.BUTTON_PRESSED;
    [SerializeField] private bool listenToThemeChanges = true;

    public ColorId ColorId => ColorId.BUTTON_NORMAL;

    private void Start()
    {
        UpdateColor();
        OnTabSelected(DefaultTab);
    }

    private void OnEnable()
    {
        if(listenToThemeChanges)
        {
            ThemeColorManager.OnThemeChanged += OnThemeChanged;
        }
    }
    private void OnDisable()
    {
        if(listenToThemeChanges)
        {
            ThemeColorManager.OnThemeChanged -= OnThemeChanged;
        }
    }

    public void OnThemeChanged(ColorTheme newTheme)
    {
        tabIdle = ThemeColorManager.Instance.GetColor(tabIdleID);
        tabHover = ThemeColorManager.Instance.GetColor(tabHoverID);
        tabActive = ThemeColorManager.Instance.GetColor(tabActiveID);
        ResetTabs();
    }
    public void UpdateColor()
    {
        OnThemeChanged(ThemeColorManager.Instance.CurrentTheme);
    }
}
