using System;
using UnityEngine;
using UnityEngine.Events;

public class SimpleDamageable : MonoBehaviour, IDamageable
{
    [Header("Color")]
    [SerializeField] private SimpleColorable colorable;

    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private bool destroyOnDeath = false;

    private float currentHealth;
    private bool isDead;
    public float CurrentHealth => Mathf.Clamp(currentHealth, 0f, maxHealth);
    public float MaxHealth => maxHealth;

    public UnityEvent OnTakeDamageUnityEvent;
    public event Action<float> OnTakeDamage;
    public event Action<Vector3, Color> OnDie;
    public static event Action<Vector3, Color> OnAnyDamageableDie;

    private void OnEnable()
    {
        currentHealth = maxHealth;
        isDead = false;
    }

    public void TakeDamage(float amount)
    {
        if (amount <= 0f || isDead || currentHealth <= 0f)
            return;

        currentHealth = Mathf.Max(0f, currentHealth - amount);

        OnTakeDamage?.Invoke(CurrentHealth);
        OnTakeDamageUnityEvent?.Invoke();

        if (currentHealth <= 0f && !isDead)
        {
            OnKilled();
        }
    }

    private void OnKilled()
    {
        if (isDead) 
            return;

        isDead = true;

        OnDie?.Invoke(transform.position, colorable.Color);
        OnAnyDamageableDie?.Invoke(transform.position, colorable.Color);

        if (destroyOnDeath)
            Destroy(gameObject);
    }
}