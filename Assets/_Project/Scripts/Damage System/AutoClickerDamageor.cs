using System;
using UnityEngine;

public class AutoClickerDamageor : BaseDamageor
{
    [Header("Références")]
    [SerializeField] private AutoClicker autoClicker;

    public static event Action<float> OnAutoClickerDamage;

    private void OnEnable()
    {
        if (autoClicker != null)
            autoClicker.OnClick += HandleClick;
    }

    private void OnDisable()
    {
        if (autoClicker != null)
            autoClicker.OnClick -= HandleClick;
    }

    private void HandleClick(Vector3 clickPosition) => TryDamage(clickPosition);
    public override void NotityDamage(float damage) => OnAutoClickerDamage?.Invoke(damage);
}
