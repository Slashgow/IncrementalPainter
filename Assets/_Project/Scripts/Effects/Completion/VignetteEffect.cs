using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VignetteEffect : SpriteCompletionEffect
{
    [SerializeField] private Color upgradeColor;
    [SerializeField, Range(0f,1f)] private float upgradeIntensity;

    [Header("Parameters")]
    [SerializeField] private bool lerpColor;
    [SerializeField] private Color startColor;
    [SerializeField] private Color endColor;
    [SerializeField] private bool shouldGoBackToOriginColor;

    [SerializeField] private bool lerpIntensity;
    [SerializeField, Range(0f, 1f)] private float startIntensity = 0f;
    [SerializeField, Range(0f, 1f)] private float endIntensity = 0.5f;

    [SerializeField] private bool lerpSmoothness;
    [SerializeField, Range(0f, 1f)] private float startSmoothness = 0f;
    [SerializeField, Range(0f, 1f)] private float endSmoothness = 0.5f;

    [SerializeField] private bool disableVignetteOnComplete;

    private Volume globalVolume;
    private Vignette vignette;

    private Tween colorTween;

    protected override void OnEnable()
    {
        base.OnEnable();

        globalVolume = FindAnyObjectByType<Volume>();

        if(globalVolume == null)
        {
            Debug.LogError("No global volume found in the scene. VignetteEffect requires a global volume to function.");
            return;
        }

        if (!GameManager.HasInstance)
        {
            this.enabled = false;
            return;
        }

        if (globalVolume.profile.TryGet(out vignette))
        {
            UpdateVignetteBasedOnRatio();
        }

        GameManager.OnStartGameState += GameManager_OnStartGameState;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        GameManager.OnStartGameState -= GameManager_OnStartGameState;

        if (colorTween != null)
            colorTween.Kill();
    }

    private void GameManager_OnStartGameState(GameManager.GameState gameState)
    {
        switch (gameState)
        {
            case GameManager.GameState.PAINT:
                vignette.color.value = startColor;
                vignette.intensity.value = Mathf.Lerp(startIntensity, endIntensity, changeCounter.Ratio);
                break;
            case GameManager.GameState.DAY_SUMMARY:
                break;
            case GameManager.GameState.UPGRADE:
                vignette.color.value = upgradeColor;
                vignette.intensity.value = upgradeIntensity;
                break;
            case GameManager.GameState.GALLERY:
                break;
        }
    }
    public override void DoEffect() => UpdateVignetteBasedOnRatio();
    private void UpdateVignetteBasedOnRatio()
    {
        if (vignette == null)
            return;

        float ratio = changeCounter.Ratio;

        if (lerpIntensity || lerpSmoothness)
            vignette.active = true;

        if (lerpIntensity)
            vignette.intensity.value = Mathf.Lerp(startIntensity, endIntensity, ratio);

        if (lerpSmoothness)
            vignette.smoothness.value = Mathf.Lerp(startSmoothness, endSmoothness, ratio);

        if (disableVignetteOnComplete && ratio >= 1f)
            vignette.active = false;
    }
}

