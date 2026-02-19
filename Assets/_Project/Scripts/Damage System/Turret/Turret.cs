using UnityEngine;
using UnityTimer;

public class Turret : MonoBehaviour
{
    [SerializeField] private Transform projectileSpawnPoint;

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
        onComplete: () => damageor.TryAttack(this.transform.position, projectileSpawnPoint.position), isLooped: true, useRealTime: false);
    }

    private void OnDestroy() => attackTimer?.Cancel();
}
