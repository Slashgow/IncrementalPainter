using System;
using UnityEngine;
using UnityEngine.Events;
using UnityTimer;

public class SimpleShieldable : MonoBehaviour, IShieldable
{
    [SerializeField] private inkolorgames.Logger logger;
    [SerializeField] private SimpleColorable colorable;
    [SerializeField] private ShieldableVisual shieldableVisual;

    [Header("Shield Configuration")]
    [SerializeField, Range(0f,300f)] private float maxShield = 100f;
    [SerializeField, Range(0f, 1f)] private float shieldArmor = 0.5f; 

    [Header("Shield Regeneration")]
    [SerializeField] private bool canRegenerate = true;
    [Tooltip("Shield points per second"), SerializeField, Range(0f,10f)] private float regenRate = 10f; 
    [Tooltip("Seconds after taking damage before regen starts"), SerializeField, Range(0f,15f)] private float regenDelay = 10f;

    private float currentShield;
    private float timeSinceLastDamage;
    private bool isRegenerating;

    public float CurrentShield => currentShield;
    public float MaxShield => maxShield;
    public float ShieldArmor => shieldArmor;
    public bool IsShieldActive => currentShield > 0f;

    public event Action<float, float> OnShieldDamaged;
    public event Action OnShieldBroken;
    public event Action<float, float> OnShieldRegenerated;

    public UnityEvent<float> OnShieldDamagedUnityEvent;
    public UnityEvent OnShieldBrokenUnityEvent;
    public UnityEvent<float> OnShieldRegeneratedUnityEvent;

    public static event Action<Vector3, Color, float> OnAnyShieldBroken; // (position, Color, scale)

    private Timer regenDisableTimer;

    public void Initialize(float maxShield, float shieldArmor, bool canRegenerate, float regenRate, float regenDelay)
    {
        this.maxShield = maxShield;
        this.shieldArmor = Mathf.Clamp01(shieldArmor);
        this.canRegenerate = canRegenerate;
        this.regenRate = regenRate;
        this.regenDelay = regenDelay;
        currentShield = maxShield;

        timeSinceLastDamage = 0f;
        isRegenerating = false;

        shieldableVisual.enabled = true;
        shieldableVisual.Initialize();
    }

    private void OnDisable()
    {
        regenDisableTimer?.Cancel();
        shieldableVisual.enabled = false;
    }

    private void Update()
    {
        if (canRegenerate && currentShield < maxShield)
        {
            timeSinceLastDamage += Time.deltaTime;

            if (timeSinceLastDamage >= regenDelay)
            {
                if (!isRegenerating)
                {
                    isRegenerating = true;
                    logger.Log($"Shield regeneration started on {gameObject.name}", this);
                }

                RegenerateShield(regenRate * Time.deltaTime);
            }
        }
    }


    public float DamageShield(float baseDamage, float shieldPenetration = 0f, float shieldBreakBonus = 0f, float shieldArmorReduction = 0f)
    {
        if (baseDamage <= 0f)
            return 0f;

        // Calculate shield penetration damage (bypasses shield entirely)
        float penetratingDamage = baseDamage * Mathf.Clamp01(shieldPenetration);
        float damageToShield = baseDamage - penetratingDamage;

        // Add shield break bonus damage
        damageToShield += shieldBreakBonus;

        // Calculate effective shield armor (reduced by armor reduction stat)
        float effectiveArmor = Mathf.Clamp01(shieldArmor - shieldArmorReduction);

        // Apply armor reduction to damage
        float finalShieldDamage = damageToShield * (1f - effectiveArmor);

        // Store shield before damage for event
        float shieldBefore = currentShield;

        // Apply damage to shield
        currentShield -= finalShieldDamage;

        // Calculate overflow damage
        float overflowDamage = 0f;
        if (currentShield < 0f)
        {
            overflowDamage = Mathf.Abs(currentShield);
            currentShield = 0f;
        }

        // Add penetrating damage to overflow
        float totalOverflowDamage = overflowDamage + penetratingDamage;

        // Reset regeneration timer
        timeSinceLastDamage = 0f;
        isRegenerating = false;

        // Fire events
        OnShieldDamaged?.Invoke(finalShieldDamage, currentShield);
        OnShieldDamagedUnityEvent?.Invoke(finalShieldDamage);

        if (shieldBefore > 0f && currentShield <= 0f)
        {
            OnShieldBroken?.Invoke();
            OnShieldBrokenUnityEvent?.Invoke();
            OnAnyShieldBroken?.Invoke(transform.position, colorable.Color, this.transform.localScale.x);

            logger.Log($"Shield broken on {gameObject.name}! Overflow damage: {totalOverflowDamage}", this);
        }

        logger.Log($"Shield damaged on {gameObject.name}: {finalShieldDamage:F1} damage " +
                     $"(Armor: {effectiveArmor * 100f:F0}%, Penetration: {shieldPenetration * 100f:F0}%, " +
                     $"Bonus: {shieldBreakBonus:F1}). Remaining: {currentShield:F1}/{maxShield:F1}. " +
                     $"Overflow: {totalOverflowDamage:F1}", this);
        

        return totalOverflowDamage;
    }

    public void RegenerateShield(float amount)
    {
        if (amount <= 0f || currentShield >= maxShield)
            return;

        float shieldBefore = currentShield;
        currentShield = Mathf.Min(currentShield + amount, maxShield);
        float actualRegen = currentShield - shieldBefore;

        if (actualRegen > 0f)
        {
            OnShieldRegenerated?.Invoke(actualRegen, currentShield);
            OnShieldRegeneratedUnityEvent?.Invoke(actualRegen);

            if (currentShield >= maxShield)
            {
                logger.Log($"Shield fully regenerated on {gameObject.name}", this);
                isRegenerating = false;
            }
        }
    }

    public void RestoreShield()
    {
        float regenAmount = maxShield - currentShield;
        if (regenAmount > 0f)
        {
            currentShield = maxShield;
            timeSinceLastDamage = 0f;
            isRegenerating = false;
            OnShieldRegenerated?.Invoke(regenAmount, currentShield);
            OnShieldRegeneratedUnityEvent?.Invoke(regenAmount);
        }
    }

    public void DisableRegeneration(float duration = -1f)
    {
        if (duration < 0f)
        {
            canRegenerate = false;
            regenDisableTimer?.Cancel();
        }
        else
        {
            bool wasRegenerating = canRegenerate;
            canRegenerate = false;
            isRegenerating = false;

            logger.Log($"Shield regeneration disabled for {duration}s on {gameObject.name}", this);
            regenDisableTimer?.Cancel();

            regenDisableTimer = Timer.Register(duration, () =>
            {
                canRegenerate = wasRegenerating;
                timeSinceLastDamage = 0f;
                regenDisableTimer = null;

                logger.Log($"Shield regeneration re-enabled on {gameObject.name}", this);
            });
        }
    }

    public void SetShieldArmor(float newArmor) => shieldArmor = Mathf.Clamp01(newArmor);
    public float GetShieldPercentage() => maxShield > 0f ? currentShield / maxShield : 0f;
}