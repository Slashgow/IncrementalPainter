using System;
using UnityEngine;

public interface IDamageable
{
    float CurrentHealth { get; }
    float MaxHealth { get; }
    void TakeDamage(float amount);
    void Die();
    event Action<float> OnTakeDamage;
    event Action<Vector3, Color> OnDie;
}
