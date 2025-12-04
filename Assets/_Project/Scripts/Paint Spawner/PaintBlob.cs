using System;
using UnityEngine;

public class PaintBlob : MonoBehaviour
{
    [SerializeField] private SpriteRenderer blobSpecificitySpriteRenderer;
    [SerializeField] private Sprite bombSprite, freezerSprite, brushSwipeSprite;

    private IDamageable damageable;
    private PaintType paintType;

    public static event Action<Vector3> OnAnyPaintBombDie;
    public static event Action<Vector3> OnAnyPaintFreezerDie;
    public static event Action<Vector3> OnAnyPaintBrushSwipeDie;

    public void Initialize(PaintType paintType)
    {
        this.paintType = paintType;

        UpdateVisualWithPaintType(paintType);
    }

    private void OnEnable()
    {
        damageable = GetComponent<IDamageable>();
        damageable.OnDie += Damageable_OnDie;
    }

    private void OnDisable() => damageable.OnDie -= Damageable_OnDie;

    private void Damageable_OnDie(Vector3 deathWorldPosition)
    {
        switch (paintType)
        {
            case PaintType.Normal:
                break;
            case PaintType.BombPaint:
                OnAnyPaintBombDie?.Invoke(deathWorldPosition);
                break;
            case PaintType.Freeze:
                OnAnyPaintFreezerDie?.Invoke(deathWorldPosition);
                break;
            case PaintType.BrushSwipe:
                OnAnyPaintBrushSwipeDie?.Invoke(deathWorldPosition);
                break;
        }
    }

    private void UpdateVisualWithPaintType(PaintType type)
    {
        switch (paintType)
        {
            case PaintType.Normal:
                blobSpecificitySpriteRenderer.gameObject.SetActive(false);
                break;
            case PaintType.BombPaint:
                blobSpecificitySpriteRenderer.gameObject.SetActive(true);
                blobSpecificitySpriteRenderer.sprite = bombSprite;
                break;
            case PaintType.Freeze:
                blobSpecificitySpriteRenderer.gameObject.SetActive(true);
                blobSpecificitySpriteRenderer.sprite = freezerSprite;
                break;
            case PaintType.BrushSwipe:
                blobSpecificitySpriteRenderer.gameObject.SetActive(true);
                blobSpecificitySpriteRenderer.sprite = brushSwipeSprite;
                break;
        }
    }
}