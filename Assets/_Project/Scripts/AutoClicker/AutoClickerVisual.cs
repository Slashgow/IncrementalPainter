using System;
using DG.Tweening;
using UnityEngine;

public class AutoClickerVisual : MonoBehaviour
{
    [SerializeField] private Collider2D circleVisualCollider;
    [SerializeField] private AutoClicker autoClicker;
    [SerializeField] private SimpleDamageor damageor;

    [SerializeField, Range(0f, 1f)] private float endScaleBonus;
    [SerializeField] private Ease ease;

    private Tween scaleTween;
    private float baseRadiusSpriteCircle;

    private void Awake()
    {
        MatchAutoClickerRadius();
        autoClicker.OnAutoClickStarted += HandleAutoClickStarted;
        autoClicker.OnAutoClickFinished += HandleAutoClickFinished;
        damageor.DamageRadiusSkillDataPerLevel.OnLevelUp += MatchAutoClickerRadius;
    }
    private void OnDestroy()
    {
        autoClicker.OnAutoClickStarted -= HandleAutoClickStarted;
        autoClicker.OnAutoClickFinished -= HandleAutoClickFinished;
        damageor.DamageRadiusSkillDataPerLevel.OnLevelUp -= MatchAutoClickerRadius;

        scaleTween?.Kill();
    }

    private void HandleAutoClickFinished() => scaleTween?.Kill();
    private void HandleAutoClickStarted()
    {
        scaleTween?.Kill();

        scaleTween = this.transform.DOScale(-endScaleBonus, autoClicker.ClickTimerInterval * 0.5f)
            .SetEase(ease)
            .SetLoops(-1, LoopType.Yoyo)
            .SetRelative(true)
            .SetUpdate(autoClicker.UseRealTime);
    }

    private void MatchAutoClickerRadius()
    {
        baseRadiusSpriteCircle = circleVisualCollider.bounds.size.x * 0.5f;
        this.transform.localScale = Vector3.one * (damageor.DamageRadius) / baseRadiusSpriteCircle;
    }
}
