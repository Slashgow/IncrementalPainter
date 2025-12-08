using UnityEngine;

public class SimpleColorable : MonoBehaviour, IColorable
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Color color;
    public Color Color => color;

    private void OnEnable()
    {
        color = PaintColorManager.Instance.GetRandomColor();

        spriteRenderer.color = color;
    }
}