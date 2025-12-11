using UnityEngine;

public static class FormatUtility
{
    public static string FormatDamage(float damage)
    {
        if (damage >= 1000000)
            return $"{damage / 1000000f:F1}M";
        if (damage >= 1000)
            return $"{damage / 1000f:F1}K";
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
