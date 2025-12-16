using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ColorAdjustmentEffect : SpriteCompletionEffect
{
    [SerializeField] private bool lerpSaturation;
    [SerializeField, Range(-100f, 100f)] private float minSaturation, maxSaturation;
    [SerializeField] private bool disableOnComplete;

    private Volume globalVolume;
    private ColorAdjustments colorAdjustement;

    protected override void OnEnable()
    {
        base.OnEnable();

        globalVolume = FindAnyObjectByType<Volume>();

        if (globalVolume.profile.TryGet(out colorAdjustement))
        {
            UpdateColorBasedOnRatio();
        }

        GameManager.OnStartGameState += GameManager_OnStartGameState;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        GameManager.OnStartGameState -= GameManager_OnStartGameState;
    }

    private void GameManager_OnStartGameState(GameManager.GameState gameState)
    {
        switch (gameState)
        {
            case GameManager.GameState.PAINT:
                colorAdjustement.saturation.value = Mathf.Lerp(minSaturation, maxSaturation, changeCounter.Ratio);
                break;
            case GameManager.GameState.DAY_SUMMARY:
                break;
            case GameManager.GameState.UPGRADE:
                colorAdjustement.saturation.value = 0f;
                break;
            case GameManager.GameState.GALLERY:
                break;
        }
    }

    public override void DoEffect() => UpdateColorBasedOnRatio();
    private void UpdateColorBasedOnRatio()
    {
        if (colorAdjustement == null)
            return;

        float ratio = changeCounter.Ratio;

        if (lerpSaturation)
            colorAdjustement.active = true;

        if (lerpSaturation)
            colorAdjustement.saturation.value = Mathf.Lerp(minSaturation, maxSaturation, ratio);

        if (disableOnComplete && ratio >= 1f)
            colorAdjustement.active = false;
    }
}

