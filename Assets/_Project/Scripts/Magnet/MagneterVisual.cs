using UnityEngine;

public class MagneterVisual : MonoBehaviour
{
    [SerializeField] private Magneter magneter;
    [SerializeField] private ParticleSystem ringParticleSystem;

    private void Start()
    {
        GameManager.OnStartGameState += GameManager_OnStartGameState;
    }

    private void OnDestroy()
    {
        if(GameManager.HasInstance)
            GameManager.OnStartGameState -= GameManager_OnStartGameState;
    }

    private void GameManager_OnStartGameState(GameManager.GameState gameState)
    {
        if (gameState != GameManager.GameState.PAINT)
            return;

        UpdateSizeRing();
    }

    private void UpdateSizeRing()
    {
        var main = ringParticleSystem.main;
        main.startSize = magneter.AttractionRadius * 2f;
    }
}
