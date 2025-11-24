using inkolorgames;
using UnityEngine;
using UnityTimer;

public class PaintSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private PoolingSystem pool;
    [SerializeField] private float spawnInterval = 1f;
    [SerializeField] private bool autoStart = true;
    [SerializeField] private int maxSpawnCount = -1;
    [SerializeField] private float zOffset = 0f;
    [SerializeField] private Transform spawnParent;
    [SerializeField] private bool useRealTime = false;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Timer spawnTimer;
    private int spawnedCount = 0;

    void Start()
    {
        if (spriteRenderer.sprite == null)
        {
            Debug.LogError("SpriteSpawner: No sprite assigned to SpriteRenderer!");
            return;
        }

        if (autoStart)
            StartSpawning();
    }

    private void OnDestroy() => StopSpawning();

    public void StartSpawning()
    {
        spawnTimer?.Cancel();
        spawnTimer = Timer.Register(spawnInterval, onComplete:SpawnObject, isLooped: true, useRealTime: useRealTime);
    }

    public void StopSpawning() => spawnTimer?.Cancel();

    public void SpawnObject()
    {
        if (maxSpawnCount >= 0 && spawnedCount >= maxSpawnCount)
        {
            StopSpawning();
            return;
        }

        Vector3 spawnPosition = SpriteUtility.GetRandomPositionInSprite(spriteRenderer.transform, spriteRenderer);
        spawnPosition.z += zOffset;
        GameObject spawned = pool.GetPrefabFromPool(spawnPosition, spawnParent, false);
        spawnedCount++;
    }

    void OnDrawGizmosSelected()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null && spriteRenderer.sprite != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(spriteRenderer.bounds.center, spriteRenderer.bounds.size);
        }
    }
}