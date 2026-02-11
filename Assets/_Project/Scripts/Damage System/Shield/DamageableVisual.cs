using UnityEngine;

public class DamageableVisual : SpriteVisualPerPercentage
{
    [SerializeField] private SimpleDamageable damageable;

    protected override void OnEnable()
    {
        base.OnEnable();

        damageable.OnTakeDamage += UpdateSprite;
        damageable.OnHeal += UpdateSprite;
        damageable.OnInitialize += UpdateSprite;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        damageable.OnTakeDamage -= UpdateSprite;
        damageable.OnHeal -= UpdateSprite;
        damageable.OnInitialize -= UpdateSprite;
    }

    private void UpdateSprite() => UpdateSpriteVisual(damageable.GetHealthPercentage());
    private void UpdateSprite(float damageAmount) => UpdateSpriteVisual(damageable.GetHealthPercentage());
    private void UpdateSprite(float arg1, float arg2) => UpdateSpriteVisual(damageable.GetHealthPercentage());
}
