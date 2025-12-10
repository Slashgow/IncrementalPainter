using System;
using inkolorgames;
using UnityEngine;

public class CurrencyHolder : BaseCostable
{
    [SerializeField, Range(0f,1f)] private float scaleMultiplier = 0.05f;
    [SerializeField, Range(0f, 3f)] private float minScale = 0.1f;
    [SerializeField, Range(0f, 3f)] private float maxScale = 0.1f;

    public static event Action<int, Vector3> OnPickUpCurrency;

    private IVacuumable vacuumable;
    private PoolingSystem pool;
    private float originalScaleSmallestCost;
    public void Initalize(int cost, PoolingSystem pool, Vector3 targetPosition)
    {
        base.Initalize(cost);
        this.pool = pool;
        vacuumable.Initialize(targetPosition);
        float scale = Mathf.Clamp(originalScaleSmallestCost * cost * scaleMultiplier, minScale, maxScale);
        this.transform.localScale = Vector3.one * scale;
    }

    private void Awake()
    {
        vacuumable = GetComponent<IVacuumable>();
        originalScaleSmallestCost = transform.localScale.x;
    }

    private void OnEnable() => vacuumable.OnVacuumCollected += Vacuumable_OnVacuumCollected;
    private void OnDisable() => vacuumable.OnVacuumCollected -= Vacuumable_OnVacuumCollected;

    private void Vacuumable_OnVacuumCollected(IVacuumer obj)
    {
        OnPickUpCurrency?.Invoke(Cost * CurrencyManager.Instance.CurrencyMultiplier, this.transform.position);
        pool.AddToPool(this.gameObject);
    }
}
