using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AutoClickerVisual : MonoBehaviour
{
    [SerializeField] private List<SpriteRenderer> circleVisuals;
    [SerializeField] private Collider2D circleVisualCollider;
    [SerializeField] private AutoClicker autoClicker;
    [SerializeField] private AutoClickerDamageor damageor;

    [SerializeField, Range(0f, 1f)] private float endScaleBonus;
    [SerializeField] private AnimationCurve curve;

    private Tween scaleTween;
    private float baseRadiusSpriteCircle;
    private float targetScale;

    private void Awake()
    {
        baseRadiusSpriteCircle = circleVisualCollider.bounds.size.x * 0.5f;
        circleVisualCollider.enabled = false;

        MatchAutoClickerRadius();
        autoClicker.OnClick += HandleClick;
        autoClicker.OnAutoClickStarted += HandleAutoClickStarted;
        autoClicker.OnAutoClickFinished += HandleAutoClickFinished;
        autoClicker.OnEnableAutoClicker += AutoClicker_OnEnableAutoClicker;
        autoClicker.OnDisableAutoClicker += AutoClicker_OnDisableAutoClicker;
        damageor.DamageRadiusSkillDataPerLevel.OnLevelUp += MatchAutoClickerRadius;
    }

    private void OnDestroy()
    {
        autoClicker.OnClick -= HandleClick;
        autoClicker.OnAutoClickStarted -= HandleAutoClickStarted;
        autoClicker.OnAutoClickFinished -= HandleAutoClickFinished;
        autoClicker.OnEnableAutoClicker -= AutoClicker_OnEnableAutoClicker;
        autoClicker.OnDisableAutoClicker -= AutoClicker_OnDisableAutoClicker;
        damageor.DamageRadiusSkillDataPerLevel.OnLevelUp -= MatchAutoClickerRadius;

        scaleTween?.Kill();
    }

    private void HandleAutoClickFinished() => scaleTween?.Kill();
    private void HandleAutoClickStarted()
    {
    }

    private void HandleClick(Vector3 position)
    {
        scaleTween?.Kill();

        this.transform.localScale = Vector3.one * targetScale;

        scaleTween = this.transform.DOScale(targetScale - endScaleBonus, autoClicker.ClickTimerInterval)
            .SetEase(curve)
            .SetUpdate(autoClicker.UseRealTime)
            .SetRecyclable(true);
    }
    private void MatchAutoClickerRadius()
    {
        scaleTween?.Kill();
        targetScale = damageor.DamageRadius / baseRadiusSpriteCircle;
        this.transform.localScale = Vector3.one * targetScale;
    }

    private void AutoClicker_OnDisableAutoClicker() => circleVisuals.ForEach(spriteRenderer => spriteRenderer.enabled = false);
    private void AutoClicker_OnEnableAutoClicker() => circleVisuals.ForEach(spriteRenderer => spriteRenderer.enabled = true);
}
