using UnityEngine;

public class SpriteVisualPerPercentage : MonoBehaviour
{
    [SerializeField] private SpriteRenderer targetSpriteRenderer;
    [SerializeField] private SpritePercentageManager spritePercentageManager;

    private float lastKnownPercentage = -1f;
    protected void UpdateSpriteVisual(float percentage)
    {
        if (Mathf.Approximately(percentage, lastKnownPercentage))
            return;

        if(percentage <= 0f)
        {
            targetSpriteRenderer.enabled = false;
            return;
        }

        if(!targetSpriteRenderer.enabled)
            targetSpriteRenderer.enabled = true;

        lastKnownPercentage = percentage;

        Sprite newSprite = spritePercentageManager.GetSpriteForPercentage(percentage);

        if (newSprite != null && newSprite != targetSpriteRenderer.sprite)
        {
            targetSpriteRenderer.sprite = newSprite;
        }
    }

    protected virtual void OnEnable()
    {
        targetSpriteRenderer.enabled = true;
    }

    protected virtual void OnDisable()
    {
        targetSpriteRenderer.enabled = false;
    }
}
