using inkolorgames.effects;
using PaintCore;
using UnityEngine;

public abstract class SpriteCompletionEffect : Effect
{
    [SerializeField] protected CwChangeCounter changeCounter;

    protected override void OnEnable()
    {
        base.OnEnable();
        changeCounter.OnUpdated += ChangeCounter_OnUpdated;
    }

    protected virtual void OnDisable() => changeCounter.OnUpdated -= ChangeCounter_OnUpdated;

    private void ChangeCounter_OnUpdated() => DoEffect();
}

