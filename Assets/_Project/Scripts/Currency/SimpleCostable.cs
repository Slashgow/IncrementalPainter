using System;
using UnityEngine;

public class SimpleCostable : MonoBehaviour, ICostable
{
    [SerializeField] private SimpleDamageable simpleDamageable;

    private int cost;
    public int Cost => cost;
    public static event Action<int> OnAnyCostableAddCurrency;

    public void Initalize(int cost) => this.cost = cost;

    private void Awake()
    {
        simpleDamageable.OnDie += SimpleDamageable_OnDie;
    }

    private void OnDestroy()
    {
        simpleDamageable.OnDie -= SimpleDamageable_OnDie;
    }
    private void SimpleDamageable_OnDie(Vector3 worldPosition, Color color) => OnAnyCostableAddCurrency?.Invoke(Cost);
}