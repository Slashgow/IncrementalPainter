using UnityEngine;

public abstract class  BossPhased : MonoBehaviour
{
    [SerializeField] protected SimpleDamageable damageable;

    [SerializeField, Range(0f, 1f)] private float phase2HealthThreshold = 0.5f;

    protected bool isPhase2 = false;

    private void OnEnable()
    {
        damageable.OnTakeDamage += CheckPhaseTransition;
        LevelManager.OnMidLevel += EnterPhase2;
    }

    private void OnDisable()
    {
        damageable.OnTakeDamage -= CheckPhaseTransition;
        LevelManager.OnMidLevel -= EnterPhase2;
    }

    private void CheckPhaseTransition(float currentHealth)
    {
        if (isPhase2)
            return;

        float healthPercentage = currentHealth / damageable.MaxHealth;

        if (healthPercentage <= phase2HealthThreshold)
            EnterPhase2();
    }


    protected virtual void EnterPhase2()
    {
        if (isPhase2)
            return;

        isPhase2 = true;
    }
}
