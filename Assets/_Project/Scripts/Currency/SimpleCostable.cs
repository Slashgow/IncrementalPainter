using System;
using UnityEngine;

public class SimpleCostable : BaseCostable
{
    [SerializeField] private SimpleDamageable simpleDamageable;

    public static event Action<Vector3, int> OnSimpleCostableDie;

    private void Awake()
    {
        simpleDamageable.OnDie += SimpleDamageable_OnDie;
    }

    private void OnDestroy()
    {
        simpleDamageable.OnDie -= SimpleDamageable_OnDie;
    }
    private void SimpleDamageable_OnDie(Vector3 worldPosition, Color color) => OnSimpleCostableDie?.Invoke(worldPosition, Cost);
}