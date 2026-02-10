using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPageBossIntroduction : UIPage
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI bossNameText;
    [SerializeField] private TextMeshProUGUI bossDescriptionText;
    [SerializeField] private Image bossImage;
    [SerializeField] private AudioSource audioSource;

    protected override void Awake()
    {
        base.Awake();

        GameManager.OnStartGameState += GameManager_OnStartGameState;

    }
    private void OnDestroy() => GameManager.OnStartGameState -= GameManager_OnStartGameState;
    private void GameManager_OnStartGameState(GameManager.GameState gameState)
    {
        if (gameState != GameManager.GameState.BOSS_INTRODUCTION)
            return;

        var currentLevelData = LevelManager.Instance.CurrentLevelData;

        if (currentLevelData == null || !currentLevelData.IsBossLevel)
            return;

        SetupPage(currentLevelData);
    }

    public void SetupPage(LevelData bossLevelData)
    {
#if !UNITY_WEBGL
        bossNameText.text = bossLevelData.BossName.GetLocalizedString();
        bossDescriptionText.text = bossLevelData.BossDescription.GetLocalizedString();
#endif

#if UNITY_WEBGL

        bossLevelData.BossName.GetLocalizedStringAsync().Completed += (handle) =>
        {
            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                 bossNameText.text = handle.Result;
            }
        };

        bossLevelData.BossDescription.GetLocalizedStringAsync().Completed += (handle) =>
        {
            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                bossDescriptionText.text = handle.Result;
            }
        };
#endif

        bossImage.sprite = bossLevelData.BossUISprite;
        audioSource.clip = bossLevelData.BossAnouncementAudioClip;
        audioSource.Play();
    }
}