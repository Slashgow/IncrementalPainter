using DG.Tweening;
using inkolorgames.effects;
using UnityEngine;

public class SpriteAlphaColorChanger : Effect
{
    [Header("Settings")]
    [SerializeField, Range(0f, 1f)] private float endAlpha;
    [SerializeField, Range(0f, 5f)] private float timeToReachEndAlpha;
    [SerializeField] private bool shouldGoBackToOriginAlpha;
    [SerializeField] private Ease easing;

    private SpriteRenderer spriteRenderer;
    private float originAlpha;
    private Tween alphaTween;
    private bool disableInAdvance;
    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originAlpha = spriteRenderer.color.a;
    }
    public override void DoEffect() => ChangeAlpha();
    public void ChangeAlpha()
    {
        if (disableInAdvance)
            return;

        Color color = spriteRenderer.color;
        color.a = originAlpha;
        spriteRenderer.color = color;
        alphaTween?.Kill();
        if (!shouldGoBackToOriginAlpha)
            alphaTween = spriteRenderer.DOFade(endAlpha, timeToReachEndAlpha).SetEase(easing);
        else
            alphaTween = spriteRenderer.DOFade(endAlpha, timeToReachEndAlpha).SetEase(easing).SetLoops(2, LoopType.Yoyo);
    }
}
