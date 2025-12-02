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

    private Tween fillTween;
    public UnityEvent OnUpdate;
    public override void DoEffect() => Fill();
    private void Fill()
    {
        fillTween?.Kill();
        float endValue = image.fillAmount;  
        image.fillAmount = 0f;
        fillTween = image.DOFillAmount(endValue, duration).SetEase(ease).OnUpdate(() => OnUpdate?.Invoke());
    }
    private void OnDestroy() => fillTween?.Kill();
}
