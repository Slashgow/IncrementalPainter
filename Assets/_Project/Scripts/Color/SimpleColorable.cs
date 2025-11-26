using UnityEngine;

public class SimpleColorable : MonoBehaviour, IColorable
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private Color color;
    public Color Color => color;

    private void OnEnable()
    {
        spriteRenderer.color = color;
    }
}