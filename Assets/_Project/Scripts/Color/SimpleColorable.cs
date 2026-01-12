using NaughtyAttributes;
using UnityEngine;

public class SimpleColorable : MonoBehaviour, IColorable
{
    [SerializeField] private bool useRandomColorOnEnable = true;
    [SerializeField, HideIf("useRandomColorOnEnable")] private Color customColor;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Color color;
    public Color Color => color;

    private void OnEnable()
    {
        if (useRandomColorOnEnable)
            color = ThemeColorManager.Instance.GetRandomPaintColor();
        else
            color = customColor;

        spriteRenderer.color = color;
    }
}