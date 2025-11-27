using UnityEngine;

public static class Helper
{
    public static Vector2 CalculateOffset(bool useRandomOffset, Vector2 offset, Vector2 randomOffsetRange)
    {
        Vector2 finalOffset = offset;
        if (useRandomOffset)
        {
            finalOffset += new Vector2(
                Random.Range(-randomOffsetRange.x, randomOffsetRange.x),
                Random.Range(-randomOffsetRange.y, randomOffsetRange.y)
            );
        }
        return finalOffset;
    }
}
