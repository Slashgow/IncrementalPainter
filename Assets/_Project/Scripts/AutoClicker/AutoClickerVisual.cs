using System;
using DG.Tweening;
using UnityEngine;

public class AutoClickerVisual : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private AutoClicker autoClicker;
    [SerializeField] private SimpleDamageor damageor;

    [SerializeField, Range(0f, 1f)] private float endAlpha;
    [SerializeField] private Ease ease;

    private Tween alphaTween;
    private float baseRadiusSpriteCircle;

    private void Awake()
    {
        MatchAutoClickerRadius();
        autoClicker.OnAutoClickStarted += HandleAutoClickStarted;
        autoClicker.OnAutoClickFinished += HandleAutoClickFinished;
    }
    private void OnDestroy()
    {
        autoClicker.OnAutoClickStarted -= HandleAutoClickStarted;
        autoClicker.OnAutoClickFinished -= HandleAutoClickFinished;

        alphaTween?.Kill();
    }

    private void HandleAutoClickFinished() => alphaTween?.Kill();
    private void HandleAutoClickStarted()
    {
        alphaTween?.Kill();

        alphaTween = this.spriteRenderer.DOFade(endAlpha, autoClicker.ClickTimerInterval * 0.5f)
            .SetEase(ease)
            .SetLoops(-1, LoopType.Yoyo)
            .SetUpdate(autoClicker.UseRealTime);
    }

    private void MatchAutoClickerRadius()
    {
        baseRadiusSpriteCircle = spriteRenderer.bounds.size.x * 0.5f;
        this.transform.localScale = Vector3.one * (damageor.DamageRadius) / baseRadiusSpriteCircle;
    }
}
