using UnityEngine;
using UnityTimer;

public class Turret : MonoBehaviour
{
    [SerializeField] private Animator turretAnimator;
    [SerializeField] private Transform projectileSpawnPoint;

    private readonly int ATTACK_HASH = Animator.StringToHash("attack");

    private TurretDamageor damageor;
    private Timer attackTimer;

    public void Initialize(TurretDamageor turretDamageor)
    {
        this.damageor = turretDamageor;
        StartAttackTimer();
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
        turretAnimator.SetTrigger(ATTACK_HASH);
        damageor.TryAttack(this.transform.position, projectileSpawnPoint.position);
    }

    private void OnDestroy() => attackTimer?.Cancel();
}
