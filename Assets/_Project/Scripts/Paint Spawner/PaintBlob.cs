using System;
using UnityEngine;

public class PaintBlob : MonoBehaviour
{
    [SerializeField] private SpriteRenderer blobSpecificitySpriteRenderer;
    [SerializeField] private Sprite bombSprite, freezerSprite, brushSwipeSprite, splitSprite, poisonSprite;

    private IDamageable damageable;
    private ISplittable splittable;

    private PaintType paintType;

    public static event Action<Vector3, Color> OnAnyPaintBombDie;
    public static event Action<Vector3> OnAnyPaintFreezerDie;
    public static event Action<Vector3, Color> OnAnyPaintBrushSwipeDie;
    public static event Action<Vector3, ISplittable> OnAnyPaintBlobSplitDie;
    public static event Action<Vector3, Color> OnAnyPaintPoisonDie;

    public void Initialize(PaintType paintType)
    {
        this.paintType = paintType;

        UpdateVisualWithPaintType(paintType);
    }

    private void OnEnable()
    {
        splittable = GetComponent<ISplittable>();
        damageable = GetComponent<IDamageable>();
        damageable.OnDie += Damageable_OnDie;
    }

    private void OnDisable() => damageable.OnDie -= Damageable_OnDie;

    private void Damageable_OnDie(Vector3 deathWorldPosition, Color color)
    {
        switch (paintType)
        {
            case PaintType.Normal:
                break;
            case PaintType.BombPaint:
                OnAnyPaintBombDie?.Invoke(deathWorldPosition, color);
                break;
            case PaintType.Freeze:
                OnAnyPaintFreezerDie?.Invoke(deathWorldPosition);
                break;
            case PaintType.BrushSwipe:
                OnAnyPaintBrushSwipeDie?.Invoke(deathWorldPosition, color);
                break;
            case PaintType.Split:
                OnAnyPaintBlobSplitDie?.Invoke(deathWorldPosition, splittable);
                break;
            case PaintType.Poison:
                OnAnyPaintPoisonDie?.Invoke(deathWorldPosition, color);
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
            case PaintType.Split:
                blobSpecificitySpriteRenderer.gameObject.SetActive(true);
                blobSpecificitySpriteRenderer.sprite = splitSprite;
                break;
            case PaintType.Poison:
                blobSpecificitySpriteRenderer.gameObject.SetActive(true);
                blobSpecificitySpriteRenderer.sprite = poisonSprite;
                break;
        }
    }
}