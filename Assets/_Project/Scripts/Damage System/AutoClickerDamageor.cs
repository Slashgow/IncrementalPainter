using UnityEngine;

public class AutoClickerDamageor : BaseDamageor
{
    [Header("Références")]
    [SerializeField] private AutoClicker autoClicker;

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
}
