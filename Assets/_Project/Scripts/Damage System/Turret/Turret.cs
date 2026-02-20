using UnityEngine;
using UnityEngine.Events;
using UnityTimer;

public class Turret : MonoBehaviour
{
    [SerializeField] private Animator turretAnimator;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private UnityEvent onFireOnce;

    private readonly int ATTACK_HASH = Animator.StringToHash("attack");

    private TurretDamageor damageor;
    private Timer attackTimer;

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
}
