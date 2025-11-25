using System;
using inkolorgames;
using UnityEngine;

public class CurrencyManager : MonoSingleton<CurrencyManager>
{
    [SerializeField] private inkolorgames.Logger logger;

    [Header("Currency Settings")]
    [SerializeField] private int startingCurrency = 0;

    private int currentCurrency;
    public int CurrentCurrency => currentCurrency;

    public event Action<int> OnCurrencyChanged;
    public event Action<int, int> OnCurrencyGained;
    public event Action<int, int> OnCurrencySpent; 

    protected override void Awake()
    {
        base.Awake();
        currentCurrency = startingCurrency;
    }

    private void OnEnable()
    {
        SimpleCostable.OnAnyCostableAddCurrency += AddCurrency;
    }

    private void OnDisable()
    {
        SimpleCostable.OnAnyCostableAddCurrency -= AddCurrency;
    }

    public void AddCurrency(int amount)
    {
        if (amount == 0)
            return;

        int oldCurrency = currentCurrency;
        currentCurrency += amount;

        OnCurrencyChanged?.Invoke(currentCurrency);

        if (amount > 0)
        {
            OnCurrencyGained?.Invoke(amount, currentCurrency);
        }
        else
        {
            OnCurrencySpent?.Invoke(Mathf.Abs(amount), currentCurrency);
        }
    }

    public bool TrySpendCurrency(int amount)
    {
        if (amount < 0)
        {
            logger.LogWarning("TrySpendCurrency called with negative amount. Use AddCurrency instead.", this);
            return false;
        }

        if (currentCurrency >= amount)
        {
            AddCurrency(-amount);
            return true;
        }

        return false;
    }

    public bool CanAfford(int amount) => CurrentCurrency >= amount;

}