using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIHPBar : MonoBehaviour
{
    [SerializeField] private Image fillAmountImage;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private bool isVisualImmediate = true;

    [SerializeField] private UnityEvent<float, float, float> OnUpdateFillAmount;

    private IDamageable damageable;
    private IHealable healable;
    private float maxHealth;
    private float previousHealth;

    public void Initialize(IDamageable damageable, IHealable healable)
    {
        this.damageable = damageable;
        this.healable = healable;
        this.maxHealth = damageable != null ? damageable.MaxHealth : 1f;
        this.previousHealth = maxHealth;

        if (damageable != null)
            damageable.OnTakeDamage += UpdateHPBar;

        if(healable != null)
            healable.OnHeal += UpdateHPBar;

        UpdateHPBar(maxHealth);
    }

    private void OnDestroy()
    {
        if (damageable != null)
            damageable.OnTakeDamage -= UpdateHPBar;

        if (healable != null)
            healable.OnHeal -= UpdateHPBar;
    }

    private void UpdateHPBar(float currentHealth)
    {
        if (isVisualImmediate)
        {
            fillAmountImage.fillAmount = currentHealth / maxHealth;
            hpText.text = $"{Mathf.FloorToInt(currentHealth)}";
        }

        OnUpdateFillAmount?.Invoke(previousHealth, currentHealth, maxHealth);

        previousHealth = currentHealth;
    }

    private void UpdateHPBar(float healAmount, float currentHealth) => UpdateHPBar(currentHealth);
}
 