using System;
public interface IShieldable
{
    float CurrentShield { get; }

    float MaxShield { get; }
    float ShieldArmor { get; }

    bool IsShieldActive { get; }

    /// <summary>
    /// Damage the shield with optional shield-specific damage modifiers
    /// </summary>
    /// <param name="baseDamage">Base damage amount</param>
    /// <param name="shieldPenetration">Percentage (0-1) of damage that bypasses shield</param>
    /// <param name="shieldBreakBonus">Additional damage specifically against shields</param>
    /// <param name="shieldArmorReduction">Reduction to shield armor for this hit</param>
    /// <returns>Overflow damage that should be applied to health</returns>
    float DamageShield(float baseDamage, float shieldPenetration = 0f, float shieldBreakBonus = 0f, float shieldArmorReduction = 0f);

    void RegenerateShield(float amount);

    event Action<float, float> OnShieldDamaged; // (damageAmount, remainingShield)
    event Action OnShieldBroken;
    event Action<float, float> OnShieldRegenerated; // (regenAmount, currentShield)
}
