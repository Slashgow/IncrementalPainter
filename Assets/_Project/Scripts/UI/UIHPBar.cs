using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHPBar : MonoBehaviour
{
    [SerializeField] private Image fillAmountImage;
    [SerializeField] private TextMeshProUGUI hpText;

    private IDamageable damageable;
    private IHealable healable;
    private float maxHealth;

    public void Initialize(IDamageable damageable, IHealable healable)
    {
        this.damageable = damageable;
        this.healable = healable;
        this.maxHealth = damageable != null ? damageable.MaxHealth : 1f;

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
        fillAmountImage.fillAmount = currentHealth / maxHealth;
        hpText.text = $"{Mathf.FloorToInt(currentHealth)}";
    }

    private void UpdateHPBar(float healAmount, float currentHealth)
    {
        fillAmountImage.fillAmount = currentHealth / maxHealth;
        hpText.text = $"{Mathf.FloorToInt(currentHealth)}";
    }
}
 