using UnityEngine;

public static class SpriteUtility
{
    public static  Vector3 GetRandomPositionInSprite(Transform transform, SpriteRenderer spriteRenderer, bool spawnOnlyInOpaquePixels = false, float alphaThreshold = 0.5f)
    {
        if (!spawnOnlyInOpaquePixels)
            return GetRandomPositionInBounds(transform, spriteRenderer.bounds);

        int maxAttempts = 50;
        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 worldPos = GetRandomPositionInBounds(transform, spriteRenderer.bounds);

            if (IsPositionOpaque(transform, worldPos, spriteRenderer, alphaThreshold))
            {
                return worldPos;
            }
        }

        return GetRandomPositionInBounds(transform, spriteRenderer.bounds);
    }

    public static Vector3 GetRandomDirectionInSprite(Transform transform, SpriteRenderer spriteRenderer, bool spawnOnlyInOpaquePixels = false, float alphaThreshold = 0.5f)
    {
        return GetRandomPositionInSprite(transform, spriteRenderer, spawnOnlyInOpaquePixels, alphaThreshold).normalized;
    }

    private static Vector3 GetRandomPositionInBounds(Transform transform, Bounds spriteBounds)
    {
        float randomX = Random.Range(spriteBounds.min.x, spriteBounds.max.x);
        float randomY = Random.Range(spriteBounds.min.y, spriteBounds.max.y);
        return new Vector3(randomX, randomY, transform.position.z);
    }

    public static bool IsPositionOpaque(Transform transform, Vector3 worldPos, SpriteRenderer spriteRenderer, float alphaThreshold)
    {
        if (spriteRenderer.sprite == null || spriteRenderer.sprite.texture == null)
            return true;

        Vector3 localPos = transform.InverseTransformPoint(worldPos);

        Sprite sprite = spriteRenderer.sprite;
        Texture2D spriteTexture = sprite.texture;
        Rect spriteRect = sprite.rect;
        Vector2 pivot = sprite.pivot;
        float pixelsPerUnit = sprite.pixelsPerUnit;

        float textureX = (localPos.x * pixelsPerUnit + pivot.x);
        float textureY = (localPos.y * pixelsPerUnit + pivot.y);

        textureX += spriteRect.x;
        textureY += spriteRect.y;

        if (textureX < spriteRect.x || textureX >= spriteRect.x + spriteRect.width ||
            textureY < spriteRect.y || textureY >= spriteRect.y + spriteRect.height)
        {
            return false;
        }

        Color pixel = spriteTexture.GetPixel((int)textureX, (int)textureY);

        return pixel.a >= alphaThreshold;
    }
}
