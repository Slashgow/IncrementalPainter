using UnityEngine;
using System.Collections.Generic;

public static class SpriteUtility
{
    private static Dictionary<SpriteRenderer, CachedSpriteData> spriteCache = new Dictionary<SpriteRenderer, CachedSpriteData>();
    private static bool isSubscribedToSceneEvents = false;

    private class CachedSpriteData
    {
        public Vector3[] opaquePositions;
        public int nextIndex;
        public float alphaThreshold;
        public int cacheSize;

        public CachedSpriteData(Vector3[] positions, float threshold, int size)
        {
            opaquePositions = positions;
            nextIndex = 0;
            alphaThreshold = threshold;
            cacheSize = size;
        }
    }

    // Static constructor to set up scene management
    static SpriteUtility()
    {
        SubscribeToSceneEvents();
    }

    private static void SubscribeToSceneEvents()
    {
        if (!isSubscribedToSceneEvents)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
            UnityEngine.SceneManagement.SceneManager.sceneUnloaded += OnSceneUnloaded;
            Application.quitting += OnApplicationQuitting;
            isSubscribedToSceneEvents = true;
        }
    }

#if UNITY_EDITOR
    private static void OnPlayModeStateChanged(UnityEditor.PlayModeStateChange state)
    {
        if (state == UnityEditor.PlayModeStateChange.ExitingPlayMode ||
            state == UnityEditor.PlayModeStateChange.EnteredEditMode)
        {
            ClearAllCaches();
        }
    }
#endif

    private static void OnSceneUnloaded(UnityEngine.SceneManagement.Scene scene)
    {
        // Clean up destroyed sprite renderers from cache
        CleanupDestroyedSprites();
    }

    private static void OnApplicationQuitting()
    {
        ClearAllCaches();
    }

    private static void CleanupDestroyedSprites()
    {
        List<SpriteRenderer> toRemove = new List<SpriteRenderer>();

        foreach (var kvp in spriteCache)
        {
            if (kvp.Key == null || kvp.Key.gameObject == null)
            {
                toRemove.Add(kvp.Key);
            }
        }

        foreach (var sprite in toRemove)
        {
            spriteCache.Remove(sprite);
        }

        if (toRemove.Count > 0)
        {
            Debug.Log($"Cleaned up {toRemove.Count} destroyed sprite caches");
        }
    }

    public static Vector3 GetRandomPositionInSprite(Transform transform, SpriteRenderer spriteRenderer, bool spawnOnlyInOpaquePixels = false, float alphaThreshold = 0.5f, int cacheSize = 500)
    {
        if (!spawnOnlyInOpaquePixels)
            return GetRandomPositionInBounds(transform, spriteRenderer.bounds);

        // Try to use cached positions first
        if (TryGetCachedPosition(spriteRenderer, alphaThreshold, cacheSize, out Vector3 cachedPos))
        {
            return cachedPos;
        }

        // Fallback: manual search with limited attempts
        int maxAttempts = 100;
        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 worldPos = GetRandomPositionInBounds(transform, spriteRenderer.bounds);

            if (IsPositionOpaque(transform, worldPos, spriteRenderer, alphaThreshold))
            {
                return worldPos;
            }
        }

        Debug.LogWarning($"Failed to find opaque pixel after {maxAttempts} attempts. Falling back to bounds.");
        return GetRandomPositionInBounds(transform, spriteRenderer.bounds);
    }

    private static bool TryGetCachedPosition(SpriteRenderer spriteRenderer, float alphaThreshold, int cacheSize, out Vector3 position)
    {
        position = Vector3.zero;

        // Safety check: ensure sprite renderer still exists
        if (spriteRenderer == null)
        {
            return false;
        }

        // Check if we have a cache for this sprite
        if (spriteCache.TryGetValue(spriteRenderer, out CachedSpriteData cache))
        {
            // Verify cache is still valid (same threshold and size)
            if (Mathf.Approximately(cache.alphaThreshold, alphaThreshold) && cache.cacheSize == cacheSize)
            {
                if (cache.opaquePositions != null && cache.opaquePositions.Length > 0)
                {
                    // Return next cached position
                    position = cache.opaquePositions[cache.nextIndex];
                    cache.nextIndex = (cache.nextIndex + 1) % cache.opaquePositions.Length;
                    return true;
                }
            }
            else
            {
                // Cache parameters changed, rebuild it
                spriteCache.Remove(spriteRenderer);
            }
        }

        // No valid cache exists, build one now
        BuildCache(spriteRenderer, alphaThreshold, cacheSize);

        // Try again with newly built cache
        if (spriteCache.TryGetValue(spriteRenderer, out cache))
        {
            if (cache.opaquePositions != null && cache.opaquePositions.Length > 0)
            {
                position = cache.opaquePositions[cache.nextIndex];
                cache.nextIndex = (cache.nextIndex + 1) % cache.opaquePositions.Length;
                return true;
            }
        }

        return false;
    }

    private static void BuildCache(SpriteRenderer spriteRenderer, float alphaThreshold, int cacheSize)
    {
        Debug.Log($"Building opaque position cache for {spriteRenderer.name} (size: {cacheSize})...");

        Vector3[] positions = GetOpaquePositions(spriteRenderer.transform, spriteRenderer, cacheSize, alphaThreshold);

        if (positions.Length > 0)
        {
            spriteCache[spriteRenderer] = new CachedSpriteData(positions, alphaThreshold, cacheSize);
            Debug.Log($"Cached {positions.Length} opaque positions for {spriteRenderer.name}");
        }
        else
        {
            Debug.LogWarning($"Failed to cache any opaque positions for {spriteRenderer.name}");
        }
    }

    // New method: Force rebuild cache (useful if sprite changes)
    public static void RebuildCache(SpriteRenderer spriteRenderer, float alphaThreshold = 0.5f, int cacheSize = 500)
    {
        if (spriteCache.ContainsKey(spriteRenderer))
        {
            spriteCache.Remove(spriteRenderer);
        }
        BuildCache(spriteRenderer, alphaThreshold, cacheSize);
    }

    // New method: Clear specific sprite cache
    public static void ClearCache(SpriteRenderer spriteRenderer)
    {
        if (spriteCache.ContainsKey(spriteRenderer))
        {
            spriteCache.Remove(spriteRenderer);
            Debug.Log($"Cleared cache for {spriteRenderer.name}");
        }
    }

    // New method: Clear all caches
    public static void ClearAllCaches()
    {
        spriteCache.Clear();
        Debug.Log("Cleared all sprite position caches");
    }

    public static Vector3? GetRandomPositionInSpriteGuaranteed(Transform transform, SpriteRenderer spriteRenderer, float alphaThreshold = 0.5f, int maxAttempts = 200)
    {
        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 worldPos = GetRandomPositionInBounds(transform, spriteRenderer.bounds);

            if (IsPositionOpaque(transform, worldPos, spriteRenderer, alphaThreshold))
            {
                return worldPos;
            }
        }

        Debug.LogError($"Failed to find opaque pixel after {maxAttempts} attempts. Returning null.");
        return null;
    }

    public static Vector3[] GetOpaquePositions(Transform transform, SpriteRenderer spriteRenderer, int sampleCount = 1000, float alphaThreshold = 0.5f)
    {
        List<Vector3> opaquePositions = new List<Vector3>();

        int attempts = 0;
        int maxAttempts = sampleCount * 10;

        while (opaquePositions.Count < sampleCount && attempts < maxAttempts)
        {
            Vector3 worldPos = GetRandomPositionInBounds(transform, spriteRenderer.bounds);

            if (IsPositionOpaque(transform, worldPos, spriteRenderer, alphaThreshold))
            {
                opaquePositions.Add(worldPos);
            }

            attempts++;
        }

        if (opaquePositions.Count < sampleCount)
        {
            Debug.LogWarning($"Only found {opaquePositions.Count} opaque positions out of {sampleCount} requested.");
        }

        return opaquePositions.ToArray();
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