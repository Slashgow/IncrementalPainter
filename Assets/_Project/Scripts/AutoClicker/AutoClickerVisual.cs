using System;
using DG.Tweening;
using UnityEngine;

public class AutoClickerVisual : MonoBehaviour
{
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
        damageor.DamageRadiusSkillDataPerLevel.OnLevelUp += MatchAutoClickerRadius;
    }
    private void OnDestroy()
    {
        autoClicker.OnClick -= HandleClick;
        autoClicker.OnAutoClickStarted -= HandleAutoClickStarted;
        autoClicker.OnAutoClickFinished -= HandleAutoClickFinished;
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
}
