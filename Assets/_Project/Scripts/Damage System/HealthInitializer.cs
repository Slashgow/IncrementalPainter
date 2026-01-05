using System;
using UnityEngine;

public class HealthInitializer : MonoBehaviour
{
    [SerializeField] private SimpleDamageable damageable;
    [SerializeField, Range(0f,5000f)] private float maxHealth = 100f;
    private void Awake()
    {
        damageable.Initialize(maxHealth);
    }
}
