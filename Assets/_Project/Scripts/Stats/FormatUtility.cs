using System.Text;
using UnityEngine;

public static class FormatUtility
{
    public static string FormatValue(UnitValue unitValue, float value)
    {
        switch (unitValue)
        {
            case UnitValue.NO_UNIT:
                return FormatValue(value);
            case UnitValue.PERCENTAGE:
                return new StringBuilder($"{value} %").ToString();
            case UnitValue.CLICK_PER_SECOND:
                return new StringBuilder($"{value} Click/s").ToString();
            case UnitValue.SECOND:
                return FormatTime(value);
            case UnitValue.METER:
                return new StringBuilder($"{value} m").ToString();
            case UnitValue.MULTIPLIER:
                return new StringBuilder($"x {value}").ToString();
            case UnitValue.BOOLEAN:
                return new StringBuilder($"{(value > 0  ? "unlocked" : "locked")}").ToString();
            case UnitValue.PERCENTAGE_0_1:
                return new StringBuilder($"{Mathf.Round(value * 100)} %").ToString();
            default:
                return FormatValue(value);
        }
    }


    public static string FormatValue(float damage)
    {
        if (damage >= 1000000)
        {
            float value = damage / 1000000f;
            return value % 1 == 0 ? $"{value:F0}M" : $"{value:F1}M";
        }
        if (damage >= 1000)
        {
            float value = damage / 1000f;
            return value % 1 == 0 ? $"{value:F0}K" : $"{value:F1}K";
        }
        return $"{damage:F0}";
    }

    public static string FormatTime(float seconds)
    {
        if (seconds >= 60)
        {
            int minutes = Mathf.FloorToInt(seconds / 60);
            int secs = Mathf.FloorToInt(seconds % 60);
            return $"{minutes}m {secs}s";
        }
        return $"{seconds:F1}s";
    }
}
