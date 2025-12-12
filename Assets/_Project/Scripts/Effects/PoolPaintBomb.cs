using inkolorgames;
using UnityEngine;

public class PoolPaintBomb : PoolInstantiatedEffect
{
    protected override void SetupPooledObject(GameObject instance, PoolingSystem pool, Color color)
    {
        base.SetupPooledObject(instance, pool);
        instance.GetComponent<BombExplosionParticle>().Initialize(color);
    }
}
