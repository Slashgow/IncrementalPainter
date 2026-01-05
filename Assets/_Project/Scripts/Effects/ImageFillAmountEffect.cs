using System;
using DG.Tweening;
using inkolorgames.effects;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ImageFillAmountEffect : Effect
{
    [SerializeField] private Image image;
    [SerializeField, Range(0f, 2f)] private float duration = 1f;
    [SerializeField] private Ease ease = Ease.InOutQuad;
    [SerializeField] private bool useRealTime = true;

    private Tween fillTween;
    public UnityEvent OnUpdate;
    public UnityEvent<string> OnUpdateFloat;
    public override void DoEffect() => Fill();
    private void Fill()
    {
        fillTween?.Kill();
        float endValue = image.fillAmount;  
        image.fillAmount = 0f;
        fillTween = image.DOFillAmount(endValue, duration).SetUpdate(useRealTime).SetEase(ease).OnUpdate(() => OnUpdate?.Invoke());
    }

    public void Fill(float startFillAmount, float targetFillAmount)
    {
        fillTween?.Kill();
        float startValue = image.fillAmount;  
        fillTween = image.DOFillAmount(targetFillAmount, duration).SetUpdate(useRealTime).SetEase(ease).OnUpdate(() => OnUpdate?.Invoke());
    }

    public void Fill(float startingHealth, float targetHealth, float maxHealth)
    {
        fillTween?.Kill();
        float startValue = startingHealth / maxHealth;
        fillTween = image.DOFillAmount(targetHealth / maxHealth, duration).SetUpdate(useRealTime).SetEase(ease)
            .OnUpdate(() => OnUpdateFloat?.Invoke(Mathf.FloorToInt(Mathf.Lerp(startingHealth, targetHealth, fillTween.ElapsedPercentage())).ToString()
            ));
    }
    private void OnDestroy() => fillTween?.Kill();
}
