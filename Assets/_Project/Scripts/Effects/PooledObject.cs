using inkolorgames;
using UnityEngine;

public class PooledObject : MonoBehaviour
{
    [SerializeField, Range(0.1f, 10f)] private float returnToPoolAfter = 2f;

    private PoolingSystem parentPool;
    private float timer;

    private void OnEnable() => timer = returnToPoolAfter;

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            ReturnToPool();
        }
    }

    public void SetPool(PoolingSystem pool) => parentPool = pool;

    public void ReturnToPool()
    {
        if (parentPool != null)
        {
            parentPool.AddToPool(gameObject);
        }
        else
        {
            Debug.LogWarning($"PooledObject {gameObject.name} has no parent pool assigned. Destroying instead.");
            Destroy(gameObject);
        }
    }

    public void SetDuration(float duration)
    {
        returnToPoolAfter = duration;
        timer = duration;
    }
}