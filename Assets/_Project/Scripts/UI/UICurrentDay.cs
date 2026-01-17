using TMPro;
using UnityEngine;
using UnityEngine.Localization;

public class UICurrentDay : MonoBehaviour, IColorChanger
{
    [SerializeField] private LocalizedString dayLocalized;
    [SerializeField] private TextMeshProUGUI dayElapsedText;
    [SerializeField] private bool showCurrentDay = false;


    private void OnEnable() => ThemeColorManager.OnThemeChanged += OnThemeChanged;
    private void OnDisable() => ThemeColorManager.OnThemeChanged -= OnThemeChanged;

    private void Awake() => LevelStatsTracker.OnDayEnded += OnDayEnded;
    private void OnDestroy() => LevelStatsTracker.OnDayEnded -= OnDayEnded;

    private void Start()
    {
        OnDayEnded(null, LevelStatsTracker.Instance.TotalLevelStats.CurrentDay);
    }

    private void OnDayEnded(DayStats dayStats, int currentDay)
    {
        dayElapsedText.color = LevelManager.Instance.GetCurrentProjectedRankColor();
        dayElapsedText.text = $"{dayLocalized.GetLocalizedString()} {(showCurrentDay ? currentDay: currentDay - 1)}";
    }

    public void OnThemeChanged(ColorTheme newTheme) => UpdateColor();
    public void UpdateColor() => dayElapsedText.color = LevelManager.Instance.GetCurrentProjectedRankColor();
}
