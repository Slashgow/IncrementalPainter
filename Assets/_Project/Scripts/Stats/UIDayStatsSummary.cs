using TMPro;
using UnityEngine;

public class UIDayStatsSummary : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI paintBlobsDestroyedValueText;
    [SerializeField] private TextMeshProUGUI autoclickerDamageValueText;
    [SerializeField] private TextMeshProUGUI bombDamageValueText;
    [SerializeField] private TextMeshProUGUI brushSwipeDamageValueText;
    [SerializeField] private TextMeshProUGUI timeAddedValueText;
    [SerializeField] private TextMeshProUGUI additionalPaintBlobsSpawnedValueText;

    private DayStats currentDayStats;

    private void Awake() => DayStatsTracker.OnDayEnded += OnDayEnded;
    private void OnDestroy() => DayStatsTracker.OnDayEnded -= OnDayEnded;

    private void OnDayEnded(DayStats dayStats)
    {
        currentDayStats = dayStats;
        UpdateDisplay(dayStats);
    }

    private void UpdateDisplay(DayStats stats)
    {
        if (stats == null)
            return;

        paintBlobsDestroyedValueText.text = stats.PaintBlobsDestroyed.ToString();
        autoclickerDamageValueText.text = FormatUtility.FormatDamage(stats.AutoClickerDamage);
        bombDamageValueText.text = FormatUtility.FormatDamage(stats.BombDamage);
        brushSwipeDamageValueText.text = FormatUtility.FormatDamage(stats.BrushSwipeDamage);
        timeAddedValueText.text = FormatUtility.FormatTime(stats.TimeAdded);
        additionalPaintBlobsSpawnedValueText.text = stats.AdditionalPaintBlobsSpawned.ToString();
    }
}
