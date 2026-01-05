using System;
using UnityEngine;

public interface IHealable
{
    float CurrentHealth { get; }
    float MaxHealth { get; }
    void Heal(float amount);
    event Action<float, float> OnHeal;
}
