using inkolorgames;
using UnityEngine;
using UnityTimer;

public class TurretProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField, Range(0f,30f)] private float speed = 10f;
    [SerializeField, Range(0f,5f)] private float maxLifetime = 5f;
    [SerializeField,Range(0f,0.3f)] private float arrivalThreshold = 0.15f;

    private TurretDamageor damageor;
    private IDamageable targetDamageable;
    private Transform target;
    private bool hasHit;
    private PoolingSystem pool;

    private Timer lifetimeTimer;

    private bool targetLost;
    private Vector3 travelDirection;
    private Vector3 spawnPosition;

    public void Initialize(TurretDamageor damageor,IDamageable damageable,Transform target, PoolingSystem pool)
    {
        this.damageor = damageor;
        this.targetDamageable = damageable;
        this.target = target;
        this.pool = pool;
        hasHit = false;
        targetLost = false;

        lifetimeTimer = Timer.Register(maxLifetime, onComplete: () => pool.AddToPool(gameObject), useRealTime: false);

        damageable.OnDie += OnTargetDie;
    }

    private void OnDisable()
    {
        lifetimeTimer?.Cancel();

        if(targetDamageable != null )
            targetDamageable.OnDie -= OnTargetDie;
    }

    private void OnTargetDie(Vector3 targetPos, Color arg2)
    {
        targetLost = true;
        travelDirection = (targetPos - transform.position).normalized;
    }

    private void Update()
    {
        if (targetLost)
        {
            transform.position += travelDirection * speed * Time.deltaTime;

            if (Vector3.Distance(transform.position, spawnPosition) >= damageor.AttackRange)
                pool.AddToPool(gameObject);

            return;
        }

        if (hasHit || target == null || targetDamageable.CurrentHealth <= 0) 
            return;

        Vector3 targetPos = target.position;
        Vector3 direction = (targetPos - transform.position).normalized;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        if (Vector3.Distance(transform.position, targetPos) <= arrivalThreshold)
            OnReachedTarget(targetPos);
    }

    private void OnReachedTarget(Vector3 position)
    {
        hasHit = true;
        damageor.TryDamage(position);
        pool.AddToPool(gameObject);
    }
}