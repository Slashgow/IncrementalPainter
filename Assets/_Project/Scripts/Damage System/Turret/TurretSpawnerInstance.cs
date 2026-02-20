using UnityEngine;
using UnityEngine.Events;
using UnityTimer;

public class TurretSpawnerInstance : MonoBehaviour
{
    [SerializeField] private Animator turretAnimator;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private UnityEvent onFire;
    [SerializeField] private UnityEvent onStartReload;
    [SerializeField] private UnityEvent onFinishReload;
    [SerializeField] private UnityEvent onBoostTriggered;

    private readonly int ATTACK_HASH = Animator.StringToHash("attack");
    private readonly int RELOAD_HASH = Animator.StringToHash("reload");

    private TurretSpawner turretSpawner;
    private Timer attackTimer;
    private Timer reloadTimer;
    PaintSpawner paintSpawner;

    private int currentMagazine;
    private bool isBoosted;
    private bool isReloading;

    public void Initialize(TurretSpawner turretSpawnerDamageor)
    {
        paintSpawner = PaintSpawner.Instance;
        this.turretSpawner = turretSpawnerDamageor;

        turretSpawner.OnFire += HandleFire;
        turretSpawner.OnStartReload += HandleStartReload;
        turretSpawner.OnFinishReload += HandleFinishReload;
        turretSpawner.OnBoostTriggered += HandleBoost;

        FillMagazine();
        StartAttackTimer();
    }

    public void RefreshAttackTimer()
    {
        attackTimer?.Cancel();
        if (!isReloading)
            StartAttackTimer();
    }

    private void StartAttackTimer()
    {
        float interval = isBoosted ? turretSpawner.BoostedAttackInterval : turretSpawner.AttackInterval;
        attackTimer = Timer.Register(interval, onComplete: TryAttack, isLooped: true, useRealTime: false);
    }

    private void StartReloadTimer()
    {
        float reloadDuration = isBoosted ? turretSpawner.BoostedReloadTime : turretSpawner.ReloadTime;
        reloadTimer = Timer.Register(reloadDuration, onComplete: FinishReload, isLooped: false, useRealTime: false);
    }

    private void TryAttack()
    {
        if (isReloading) 
            return;

        bool fired = turretSpawner.TryFire(ref currentMagazine, ref isBoosted);

        if (fired)
        {
            for (int i = 0; i < turretSpawner.PaintBlobsPerFire; i++)
                FireBlob();

            if (turretAnimator != null)
                turretAnimator.SetTrigger(ATTACK_HASH);
        }

        if (currentMagazine <= 0 && !isReloading)
            BeginReload();
    }

    private void FireBlob()
    {
        // Ask PaintSpawner for a random canvas position – this becomes the blob's landing target.
        // We temporarily override the spawn point so the blob starts at the turret barrel,
        // then PaintBlobLauncher steers it to the canvas position.
        Vector3 spawnOrigin = projectileSpawnPoint.position;
        Vector3 targetPosition = paintSpawner.GetSpawnPosition();
        PaintType paintType = paintSpawner.GetWeightedPaintType();

        GameObject blobObj = paintSpawner.SpawnPaintBlob(spawnOrigin, paintType);

        if (blobObj != null && blobObj.TryGetComponent<PaintBlobLauncher>(out var launcher))
        {
            launcher.enabled = true; 
            launcher.Launch(targetPosition);
        }
            
    }

    private void BeginReload()
    {
        isReloading = true;
        isBoosted = false; // boost resets on reload
        attackTimer?.Cancel();
        turretSpawner.NotifyStartReload();
        StartReloadTimer();
    }

    private void FinishReload()
    {
        FillMagazine();
        isReloading = false;
        turretSpawner.NotifyFinishReload();
        StartAttackTimer();
    }

    private void FillMagazine()
    {
        currentMagazine = turretSpawner.MagazineCapacity;
    }

    private void HandleFire()
    {
        onFire?.Invoke();
    }

    private void HandleStartReload()
    {
        onStartReload?.Invoke();
        if (turretAnimator != null)
            turretAnimator.SetTrigger(RELOAD_HASH);
    }

    private void HandleFinishReload()
    {
        onFinishReload?.Invoke();
    }

    private void HandleBoost()
    {
        onBoostTriggered?.Invoke();
        // Re-start attack timer at the boosted interval mid-flight
        attackTimer?.Cancel();
        StartAttackTimer();
    }

    private void OnDestroy()
    {
        attackTimer?.Cancel();
        reloadTimer?.Cancel();

        if (turretSpawner == null) 
            return;

        turretSpawner.OnFire -= HandleFire;
        turretSpawner.OnStartReload -= HandleStartReload;
        turretSpawner.OnFinishReload -= HandleFinishReload;
        turretSpawner.OnBoostTriggered -= HandleBoost;
    }
}
