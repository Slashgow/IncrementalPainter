using TMPro;
using UnityEngine;
using UnityEngine.Localization;

public class UICurrentDay : MonoBehaviour
{
    [SerializeField] private LocalizedString dayLocalized;
    [SerializeField] private TextMeshProUGUI dayElapsedText;
    [SerializeField] private bool showCurrentDay = false;

    private void Awake() => LevelStatsTracker.OnDayEnded += OnDayEnded;
    private void OnDestroy() => LevelStatsTracker.OnDayEnded -= OnDayEnded;

    private void Start()
    {
        OnDayEnded(null, LevelStatsTracker.Instance.TotalLevelStats.CurrentDay);
    }

    private void OnDayEnded(DayStats dayStats, int currentDay)
    {
        dayElapsedText.text = $"{dayLocalized.GetLocalizedString()} {(showCurrentDay ? currentDay: currentDay - 1)}";
    }
}
