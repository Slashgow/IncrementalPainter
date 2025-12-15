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
    [SerializeField] private TextMeshProUGUI currencyGainedValueText;

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

        paintBlobsDestroyedValueText.text = FormatUtility.FormatValue(stats.PaintBlobsDestroyed);
        autoclickerDamageValueText.text = FormatUtility.FormatValue(stats.AutoClickerDamage);
        bombDamageValueText.text = FormatUtility.FormatValue(stats.BombDamage);
        brushSwipeDamageValueText.text = FormatUtility.FormatValue(stats.BrushSwipeDamage);
        timeAddedValueText.text = FormatUtility.FormatTime(stats.TimeAdded);
        additionalPaintBlobsSpawnedValueText.text = FormatUtility.FormatValue(stats.AdditionalPaintBlobsSpawned);
        currencyGainedValueText.text = FormatUtility.FormatValue(stats.CurrencyGained);
    }
}
