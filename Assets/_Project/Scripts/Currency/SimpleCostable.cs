using System;
using UnityEngine;

public class SimpleCostable : MonoBehaviour, ICostable
{
    [SerializeField, Range(0,100)] private int cost;
    public int Cost => cost;
    public static event Action<int> OnAnyCostableAddCurrency;

    private void Awake()
    {
        SimpleDamageable.OnAnyDamageableDie += SimpleDamageable_OnAnyDamageableDie;
    }

    private void OnDestroy()
    {
        SimpleDamageable.OnAnyDamageableDie -= SimpleDamageable_OnAnyDamageableDie;
    }
    private void SimpleDamageable_OnAnyDamageableDie(Vector3 worldPosition, Color color) => OnAnyCostableAddCurrency?.Invoke(Cost);
}