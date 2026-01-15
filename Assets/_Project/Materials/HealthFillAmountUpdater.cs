using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class HealthFillAmountUpdater : MonoBehaviour
{
    [SerializeField] private SimpleDamageable damageable;
    [SerializeField] private SpriteRenderer targetRenderer;
    [SerializeField] private bool updateOnStart = true;
    [SerializeField] private string fillAmountPropertyName = "_FillAmount";

    private Material materialInstance;
    private int fillAmountPropertyID;

    private void Awake()
    {
        if (targetRenderer != null && targetRenderer.material != null)
        {
            materialInstance = targetRenderer.material;
            targetRenderer.material = materialInstance;
        }

        fillAmountPropertyID = Shader.PropertyToID(fillAmountPropertyName);
    }

    private void Start()
    {
        if (updateOnStart)
            UpdateFillAmount();
    }

    private void OnEnable()
    {
        damageable.OnTakeDamage += OnHealthChanged;
        damageable.OnHeal += OnHealed;
    }

    private void OnDisable()
    {
       damageable.OnTakeDamage -= OnHealthChanged;
       damageable.OnHeal -= OnHealed; 
    }

    private void OnHealthChanged(float currentHealth) => UpdateFillAmount();
    private void OnHealed(float healAmount, float currentHealth) => UpdateFillAmount();

    public void UpdateFillAmount()
    {
        float fillAmount = CalculateFillAmount();
        materialInstance.SetFloat(fillAmountPropertyID, fillAmount);
    }

    private float CalculateFillAmount()
    {
        if (damageable.MaxHealth <= 0f)
            return 0f;

        return Mathf.Clamp01(damageable.CurrentHealth / damageable.MaxHealth);
    }
    private void OnDestroy()
    {
        if (materialInstance != null)
        {
            Destroy(materialInstance);
        }
    }
}