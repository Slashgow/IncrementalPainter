using inkolorgames.effects;
using UnityEngine;
using UnityTimer;

public class SpriteSwapper : Effect
{
    [SerializeField] private SpriteRenderer targetSpriteRenderer;
    [SerializeField] private Sprite newSprite;
    [SerializeField, Range(0f, 10f)] private float swapDuration = 1f;

    private Sprite oldSprite;
    private Timer swapTimer;

    protected override void OnEnable()
    {
        oldSprite = targetSpriteRenderer.sprite;
        base.OnEnable();
    }
    public override void DoEffect()
    {
        swapTimer?.Cancel();
        swapTimer = Timer.Register(swapDuration, onComplete: ShowOriginalSprite);
        targetSpriteRenderer.sprite = newSprite;
    }

    public void ShowOriginalSprite() => targetSpriteRenderer.sprite = oldSprite;
}
