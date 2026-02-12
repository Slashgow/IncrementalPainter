using DG.Tweening;
using inkolorgames;
using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoSingleton<MusicManager>
{
    [SerializeField] private inkolorgames.Logger logger;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource secondaryAudioSource;

    [Header("Paint Music")]
    [SerializeField] private AudioClip paintMusic;
    [SerializeField, Range(0f, 2f)] private float paintMusicCrossfadeDuration = 1f;

    [Header("Upgrade Music")]
    [SerializeField] private AudioClip upgradeMusic;
    [SerializeField, Range(0f,2f)] private float upgradeMusicCrossfadeDuration = 1f;

    [SerializeField] private AudioClip bossMusic, bossUpgradeMusic;
    [SerializeField] private Ease fadeEase = Ease.InOutQuad;

    private Tween volumeFadeOutTween;
    private Tween volumeFadeInTween;
    public AudioSource CurrentAudioSource { get; private set; }

    private void OnEnable() => GameManager.OnStartGameState += GameManager_OnStartGameState;
    private void OnDisable() => GameManager.OnStartGameState -= GameManager_OnStartGameState;


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

    public void CrossfadeToClip(AudioClip newClip, float duration, bool loopNext)
    {
        if (newClip == null)
        {
            logger.Log("Cannot crossfade to null clip", this);
            return;
        }

        if (CurrentAudioSource == null)
        {
            // First time playing - no crossfade needed
            CurrentAudioSource = audioSource;
            CurrentAudioSource.clip = newClip;
            CurrentAudioSource.volume = 1f;
            CurrentAudioSource.pitch = 1f;
            CurrentAudioSource.Play();
            logger.Log($"Started playing music: {newClip.name}", this);
            return;
        }

        volumeFadeOutTween?.Kill();
        volumeFadeInTween?.Kill();

        AudioSource fadeOutSource = CurrentAudioSource;
        AudioSource fadeInSource = CurrentAudioSource == audioSource ? secondaryAudioSource : audioSource;

        fadeInSource.clip = newClip;
        fadeInSource.volume = 0f;
        fadeInSource.pitch = CurrentAudioSource.pitch;

        if (loopNext)
            fadeInSource.loop = true;

        fadeInSource.Play();

        volumeFadeOutTween = fadeOutSource.DOFade(0f, duration)
            .SetEase(fadeEase)
            .SetUpdate(true)
            .OnComplete(() => fadeOutSource.Stop());

        volumeFadeInTween = fadeInSource.DOFade(1f, duration)
            .SetEase(fadeEase)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                CurrentAudioSource = fadeInSource;
                logger.Log($"Crossfaded to clip: {newClip.name} over {duration}s", this);
            });

        logger.Log($"Starting crossfade to clip: {newClip.name} over {duration}s", this);
    }
}
