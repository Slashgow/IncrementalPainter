using System;
using UnityEngine;

public class AutoClickerDamageor : BaseDamageor
{
    [Header("Références")]
    [SerializeField] private AutoClicker autoClicker;

    public override float DamageRadius => autoClicker != null && autoClicker.isBoostingRadius ? 
        base.DamageRadius * autoClicker.RadiusSizeMultiplier : 
        base.DamageRadius;

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

    public void HandleClick(Vector3 clickPosition) => TryDamage(clickPosition);
    public override void NotityDamage(float damage) => OnAutoClickerDamage?.Invoke(damage);
}
