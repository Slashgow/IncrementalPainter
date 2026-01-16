using UnityEngine;

public class MagneterVisual : MonoBehaviour
{
    [SerializeField] private Magneter magneter;
    [SerializeField] private ParticleSystem ringParticleSystem;

    private void OnEnable() => GameManager.OnStartGameState += GameManager_OnStartGameState;
    private void OnDisable() => GameManager.OnStartGameState -= GameManager_OnStartGameState;

    private void GameManager_OnStartGameState(GameManager.GameState gameState)
    {
        if (gameState != GameManager.GameState.PAINT)
            return;

        UpdateSizeRing();
    }

    public void UpdateSizeRing()
    {
        var main = ringParticleSystem.main;
        main.startSize = magneter.AttractionRadius * 2f;
    }
}
