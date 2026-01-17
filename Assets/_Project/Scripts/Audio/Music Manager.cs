using inkolorgames;
using UnityEngine;

public class MusicManager : BaseMusicManager
{
    [Header("Paint Music")]
    [SerializeField] private AudioClip paintMusic;
    [SerializeField, Range(0f, 2f)] private float paintMusicCrossfadeDuration = 1f;

    [Header("Upgrade Music")]
    [SerializeField] private AudioClip upgradeMusic;
    [SerializeField, Range(0f,2f)] private float upgradeMusicCrossfadeDuration = 1f;

    [SerializeField] private AudioClip bossMusic, bossUpgradeMusic;

    private void OnEnable() => GameManager.OnStartGameState += GameManager_OnStartGameState;
    private void OnDisable() => GameManager.OnStartGameState -= GameManager_OnStartGameState;

    protected override void Awake()
    {
        base.Awake();

        if (!LevelManager.Instance.CurrentLevelData.IsBossLevel)
            PlayMusicAtIndex(0);
        else
            PlayMusicAtIndex(1);
    }

    private void GameManager_OnStartGameState(GameManager.GameState gamestate)
    {
        switch (gamestate)
        {
            case GameManager.GameState.PAINT:
                if(LevelManager.Instance.CurrentLevelData.IsBossLevel)
                    CrossfadeToClip(bossMusic, paintMusicCrossfadeDuration, true);
                else
                    CrossfadeToClip(paintMusic, paintMusicCrossfadeDuration, true);
                break;
            case GameManager.GameState.DAY_SUMMARY:
                if (LevelManager.Instance.CurrentLevelData.IsBossLevel)
                    CrossfadeToClip(bossUpgradeMusic, upgradeMusicCrossfadeDuration, true);
                else
                    CrossfadeToClip(upgradeMusic, upgradeMusicCrossfadeDuration, true);
                break;
            case GameManager.GameState.UPGRADE:
                break;
            case GameManager.GameState.GALLERY:
                break;
        }
    }
}
