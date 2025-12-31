using UnityEngine;

public interface IHealer
{
    float HealAmount { get; }
    void PerformHeal(IHealable target);
}
