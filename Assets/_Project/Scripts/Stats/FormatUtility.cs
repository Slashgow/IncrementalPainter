using UnityEngine;

public static class FormatUtility
{
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
