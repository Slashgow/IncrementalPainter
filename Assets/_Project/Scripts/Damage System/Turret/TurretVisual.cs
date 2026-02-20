using UnityEngine;

public class TurretVisual : MonoBehaviour
{
    [SerializeField] private TurretDamageInstance turret;
    [SerializeField] private ParticleSystem ringParticleSystem;

    private void OnEnable() => GameManager.OnStartGameState += GameManager_OnStartGameState;
    private void OnDisable() => GameManager.OnStartGameState -= GameManager_OnStartGameState;
    private void Start()
    {
        UpdateSizeRing();
    }
    private void GameManager_OnStartGameState(GameManager.GameState gameState)
    {
        if (gameState != GameManager.GameState.PAINT)
            return;

        UpdateSizeRing();
    }

    public void UpdateSizeRing()
    {
        var main = ringParticleSystem.main;
        float scale = ringParticleSystem.transform.lossyScale.x;
        main.startSize = (turret.AttackRange * 2f) * scale;

    }
}
