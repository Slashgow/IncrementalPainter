using UnityEngine;
using UnityEngine.Events;
using UnityTimer;

public class TurretDamageInstance : MonoBehaviour
{
    [SerializeField] private Animator turretAnimator;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private UnityEvent onFireOnce;

    private readonly int ATTACK_HASH = Animator.StringToHash("attack");

    private TurretDamageor damageor;
    private Timer attackTimer;
    public float AttackRange => damageor.AttackRange;

    public void Initialize(TurretDamageor turretDamageor)
    {
        this.damageor = turretDamageor;

        this.damageor.OnFireOnce += LaunchAttackAnim;

        StartAttackTimer();
    }

    private void LaunchAttackAnim()
    {
        onFireOnce?.Invoke();
        turretAnimator.SetTrigger(ATTACK_HASH);
    }

    public void RefreshAttackTimer()
    {
        attackTimer?.Cancel();
        StartAttackTimer();
    }

    private void StartAttackTimer()
    {
        attackTimer = Timer.Register(damageor.AttackInterval,
        onComplete: () => TryAttack(), isLooped: true, useRealTime: false);
    }

    private void TryAttack()
    {
        damageor.TryAttack(this.transform.position, projectileSpawnPoint.position);
    }

    private void OnDestroy()
    {
        attackTimer?.Cancel();
        this.damageor.OnFireOnce -= LaunchAttackAnim;
    }

    private void OnDrawGizmosSelected()
    {
        if(damageor == null)
            return;

        if (damageor.AttackRangeSkillDataPerLevel == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
    }
}
