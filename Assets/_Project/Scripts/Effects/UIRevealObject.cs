using DG.Tweening;
using inkolorgames.effects;
using UnityEngine;

public class UIRevealObject : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private EffectApplier effectApplier;

    public CanvasGroup CanvasGroup => canvasGroup;
    public EffectApplier EffectApplier => effectApplier;

    private Tween tween;
    public Tween Show(float fadeDuration)
    {
        tween?.Kill();
        tween = canvasGroup.DOFade(1f, fadeDuration);
        return tween;
    }

    public void DoEffects() => effectApplier.DoEffects();
}
