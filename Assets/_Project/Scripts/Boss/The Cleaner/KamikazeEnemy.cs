using System;
using UnityEngine;
using UnityTimer;

public class KamikazeEnemy : EraserEnemy
{
    [SerializeField] private bool eraseOnDie;
    [SerializeField, Range(0f, 10f)] private float timeBeforeExploding = 5f;
    [SerializeField, Range(0f,1f)] private float timeBetweenNotifications = 0.5f;

    private Timer explosionTimer;
    private IDamageable damageable;
    private float timeSinceLastNotification;

    public event Action<float> OnExplosionWarning;
    public float TimeBeforeExploding => timeBeforeExploding;
    private void Awake() => damageable = GetComponent<IDamageable>();

    protected void OnEnable()
    {
       explosionTimer = Timer.Register(timeBeforeExploding, onComplete: () => Explode(), 
           useRealTime: false, onUpdate: (timeElapsed) => ExplosionWarning(timeElapsed));
    }

    private void ExplosionWarning(float timeElapsed)
    {
        timeSinceLastNotification += Time.deltaTime;

        if (timeSinceLastNotification >= timeBetweenNotifications)
        {
            float timeRemaining = timeBeforeExploding - timeElapsed;
            float percentage = timeRemaining / timeBeforeExploding;
            OnExplosionWarning?.Invoke(percentage);
            timeSinceLastNotification = 0f;
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        explosionTimer?.Cancel();
    }

    private void Explode()
    {
        ErasePaintInArea();
        damageable.Die();
    }
}
