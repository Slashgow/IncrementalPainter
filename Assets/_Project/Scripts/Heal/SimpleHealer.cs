using UnityEngine;

public class SimpleHealer : MonoBehaviour, IHealer
{
    [Header("Heal Settings")]
    [SerializeField] private float healAmount = 10f;

    public float HealAmount => healAmount;

    public void SetHealAmount(float amount) => healAmount = amount;

    public void PerformHeal(IHealable target)
    {
        if (target == null)
            return;

        target.Heal(healAmount);
    }
}