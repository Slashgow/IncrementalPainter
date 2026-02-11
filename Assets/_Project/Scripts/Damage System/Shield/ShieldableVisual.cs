using UnityEngine;


public class ShieldableVisual : SpriteVisualPerPercentage
{
    [SerializeField] private SimpleShieldable shieldable;

    protected override void OnEnable()
    {
        base.OnEnable();
        shieldable.OnShieldDamaged += OnShieldChanged;
        shieldable.OnShieldRegenerated += OnShieldChanged;
        shieldable.OnShieldBroken += OnShieldChanged;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        shieldable.OnShieldDamaged -= OnShieldChanged;
        shieldable.OnShieldRegenerated -= OnShieldChanged;
        shieldable.OnShieldBroken -= OnShieldChanged;
    }

    public void Initialize()
    {
        OnShieldChanged();
    }

    private void OnShieldChanged(float amount, float newShield) => UpdateSpriteVisual(shieldable.GetShieldPercentage());
    private void OnShieldChanged() => UpdateSpriteVisual(shieldable.GetShieldPercentage());

    
}
