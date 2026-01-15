using DG.Tweening;
using inkolorgames.effects;
using UnityEngine;

public class SpriteColorChanger : Effect
{
    [Header("Settings")]
    [SerializeField] private Color endColor;
    [SerializeField, Range(0f, 5f)] private float timeToReachEndColor;
    [SerializeField] private bool shouldGoBackToOriginColor;
    [SerializeField] private Ease easing;

    private SpriteRenderer spriteRenderer;
    private SimpleColorable simpleColorable;
    private Color originColor;
    private Tween colorTween;

    private bool disableInAdvance;

    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        simpleColorable = GetComponent<SimpleColorable>();
        originColor = spriteRenderer.color;
    }

    public override void DoEffect() => ChangeColor();

    public void ChangeColor()
    {
        if (disableInAdvance)
            return;

        spriteRenderer.color = simpleColorable != null ? simpleColorable.Color : originColor;

        colorTween?.Kill();

        if (!shouldGoBackToOriginColor)
            colorTween = spriteRenderer.DOColor(endColor, timeToReachEndColor).SetEase(easing);
        else
            colorTween = spriteRenderer.DOColor(endColor, timeToReachEndColor).SetEase(easing).SetLoops(2, LoopType.Yoyo);
    }


    public void KillTween(bool disableInAdvance)
    {
        if (colorTween != null)
            colorTween.Kill();

        this.disableInAdvance = disableInAdvance;
    }

    private void OnDestroy() => colorTween?.Kill();
}
