using System;
using UnityEngine;

public class SimpleDamageable : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float startHealth = -1f; // -1 => initialise à maxHealth
    [SerializeField] private bool destroyOnDeath = false;

    private float currentHealth;
    public float CurrentHealth => Mathf.Clamp(currentHealth, 0f, MaxHealth);
    public float MaxHealth => Mathf.Clamp(currentHealth, 0f, maxHealth);

    public event Action<float> OnDamage;

    private void Awake()
    {
        if (startHealth <= 0f)
            currentHealth = maxHealth;
        else
            currentHealth = Mathf.Clamp(startHealth, 0f, maxHealth);
    }

    public void TakeDamage(float amount)
    {
        if (amount <= 0f || currentHealth <= 0f) 
            return;

        currentHealth -= amount;
        OnDamage?.Invoke(CurrentHealth);

        if (CurrentHealth <= 0f)
            OnKilled();
    }

    private void OnKilled()
    {
        if (destroyOnDeath)
            Destroy(gameObject);
    }
}