using System;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

public class AutoClickerVisual : MonoBehaviour
{
    [SerializeField] private List<SpriteRenderer> circleVisuals;
    [SerializeField] private Collider2D circleVisualCollider;
    [SerializeField] private bool useAutoClickerValue = true;
    [SerializeField, ShowIf("useAutoClickerValue")] private AutoClicker autoClicker;
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

        if(damageor != null)
        {
            damageor.DamageRadiusSkillDataPerLevel.OnLevelUp += MatchAutoClickerRadius;
            damageor.DamageRadiusSkillDataPerLevel.OnLevelDown += MatchAutoClickerRadius;
        }
        else
        {
            AutoClickerAutonomousManager.Instance.AutoClickerDamageor.DamageRadiusSkillDataPerLevel.OnLevelUp += MatchAutoClickerRadius;
            AutoClickerAutonomousManager.Instance.AutoClickerDamageor.DamageRadiusSkillDataPerLevel.OnLevelDown += MatchAutoClickerRadius;
        }

        autoClicker.OnBoostRadiusStart += MatchAutoClickerRadius;
        autoClicker.OnBoostRadiusEnd += MatchAutoClickerRadius;
    }

    private void OnDestroy()
    {
        autoClicker.OnClick -= HandleClick;
        autoClicker.OnAutoClickStarted -= HandleAutoClickStarted;
        autoClicker.OnAutoClickFinished -= HandleAutoClickFinished;
        autoClicker.OnEnableAutoClicker -= AutoClicker_OnEnableAutoClicker;
        autoClicker.OnDisableAutoClicker -= AutoClicker_OnDisableAutoClicker;
        if (damageor != null)
        {
            damageor.DamageRadiusSkillDataPerLevel.OnLevelUp -= MatchAutoClickerRadius;
            damageor.DamageRadiusSkillDataPerLevel.OnLevelDown -= MatchAutoClickerRadius;
        }
        else
        {
            AutoClickerAutonomousManager.Instance.AutoClickerDamageor.DamageRadiusSkillDataPerLevel.OnLevelUp -= MatchAutoClickerRadius;
            AutoClickerAutonomousManager.Instance.AutoClickerDamageor.DamageRadiusSkillDataPerLevel.OnLevelDown -= MatchAutoClickerRadius;
        }
        autoClicker.OnBoostRadiusStart -= MatchAutoClickerRadius;
        autoClicker.OnBoostRadiusEnd -= MatchAutoClickerRadius;

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

        float duration = useAutoClickerValue ? autoClicker.ClickTimerInterval : AutoClickerAutonomousManager.Instance.ClickTimerInterval;
        scaleTween = this.transform.DOScale(targetScale - endScaleBonus, duration)
            .SetEase(curve)
            .SetUpdate(autoClicker.UseRealTime)
            .SetRecyclable(true);
    }
    private void MatchAutoClickerRadius()
    {
        scaleTween?.Kill();
        targetScale = damageor != null ? 
            damageor.DamageRadius / baseRadiusSpriteCircle : 
            AutoClickerAutonomousManager.Instance.AutoClickerDamageor.DamageRadius / baseRadiusSpriteCircle;
        this.transform.localScale = Vector3.one * targetScale;
    }

    private void AutoClicker_OnDisableAutoClicker() => circleVisuals.ForEach(spriteRenderer => spriteRenderer.enabled = false);
    private void AutoClicker_OnEnableAutoClicker() => circleVisuals.ForEach(spriteRenderer => spriteRenderer.enabled = true);
}
