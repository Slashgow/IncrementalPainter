using System;
using inkolorgames;
using UnityEngine;
using UnityEngine.Events;

public class CurrencyManager : MonoSingleton<CurrencyManager>
{
    [SerializeField] private inkolorgames.Logger logger;
    [SerializeField] private PoolingSystem currencyPool;

    [Header("Currency Settings")]
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> currencyMultiplierPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> CurrencyMultiplierPerLevel => currencyMultiplierPerLevel;

    [SerializeField] private int startingCurrency = 0;

    private int currentCurrency;
    public int CurrentCurrency => currentCurrency;
    public int CurrencyMultiplier => Mathf.FloorToInt(currencyMultiplierPerLevel.GetCurrentLevelData());

    public event Action<int> OnCurrencyChanged;
    public event Action<int, int> OnCurrencyGained;
    public event Action<int, int> OnCurrencySpent;

    public UnityEvent OnCurrencyGainedUnity;

    protected override void Awake()
    {
        base.Awake();
        currentCurrency = startingCurrency;
    }

    private void OnEnable()
    {
        CurrencyHolder.OnPickUpCurrency += OnPickUpCurrency;
        SimpleCostable.OnSimpleCostableDie += SpawnCurrencyHolder;
    }

    private void OnDisable()
    {
        CurrencyHolder.OnPickUpCurrency -= OnPickUpCurrency;
        SimpleCostable.OnSimpleCostableDie -= SpawnCurrencyHolder;
    }

    public void SpawnCurrencyHolder(Vector3 deathWorldPosition, int cost)
    {
        GameObject currencyGOInstance = currencyPool.GetPrefabFromPool();
        currencyGOInstance.GetComponent<CurrencyHolder>().Initalize(cost, currencyPool, deathWorldPosition);
        currencyGOInstance.transform.position = deathWorldPosition;
    }

    private void OnPickUpCurrency(int amount, Vector3 worldPosition) => AddCurrency(amount);

    public void AddCurrency(int amount)
    {
        if (amount == 0)
            return;

        int oldCurrency = currentCurrency;
        currentCurrency += amount * CurrencyMultiplier;

        OnCurrencyChanged?.Invoke(currentCurrency);

        if (amount > 0)
        {
            logger.Log($"Gained {amount} currency. Total: {currentCurrency}", this);
            OnCurrencyGained?.Invoke(amount, currentCurrency);
            OnCurrencyGainedUnity?.Invoke();
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