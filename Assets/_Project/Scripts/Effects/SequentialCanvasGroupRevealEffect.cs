using System.Collections.Generic;
using DG.Tweening;
using inkolorgames.effects;
using UnityEngine;
using UnityEngine.Events;

public class SequentialCanvasGroupRevealEffect : Effect
{
    [Header("Canvas Groups")]
    [SerializeField] private List<UIRevealObject> revealObjects = new List<UIRevealObject>();

    [Header("Timing")]
    [SerializeField, Range(0f,1f)] private float delayBetweenReveals = 0.3f;
    [SerializeField, Range(0f,1f)] private float fadeDuration = 0.5f;

    [Header("Events")]
    [SerializeField] private UnityEvent onElementRevealed = new UnityEvent();

    private Sequence sequence;

    private void Start() => HideCanvasGroups();
    public void HideCanvasGroups()
    {
        foreach (var revealObject in revealObjects)
        {
            if (revealObject != null)
                revealObject.CanvasGroup.alpha = 0f;
        }
    }

    private void OnDestroy() => sequence?.Kill();
    public override void DoEffect()
    {
        sequence?.Kill();

        sequence = DOTween.Sequence();
        sequence.SetUpdate(true);

        for (int i = 0; i < revealObjects.Count; i++)
        {
            var revealedObject = revealObjects[i];
            if (revealedObject == null) 
                continue;

            revealedObject.CanvasGroup.alpha = 0f;

            if (i > 0)
                sequence.AppendInterval(delayBetweenReveals);

            sequence.Append(revealedObject.Show(fadeDuration));
            sequence.AppendCallback(() => {
                revealedObject.DoEffects();
                onElementRevealed?.Invoke();
                });
        }
    }
}