using System;
using inkolorgames;
using PaintCore;
using UnityEngine;

public class ArtGalleryColorManager : MonoSingleton<ArtGalleryColorManager>
{
    [SerializeField] private CwModifyColorRandom modifyColorRandom;
    [SerializeField] private Color defaultColor = Color.black;

    private Color currentColor;

    public static event Action<Color> OnColorChanged;
    public static event Action<CwModifyColorRandom> OnApplyColorRandomModifier;
    public static event Action<CwModifyColorRandom> OnRemoveColorRandomModifier;

    private void Start()
    {
        SetCurrentColor(defaultColor);
    }

    public void SetCurrentColor(Color color)
    {
        currentColor = color;
        OnColorChanged?.Invoke(currentColor);
    }

    public void ToggleColorModifier(bool enable)
    {
        if(enable)
        {
            OnApplyColorRandomModifier?.Invoke(modifyColorRandom);
        }
        else
        {
            OnRemoveColorRandomModifier?.Invoke(modifyColorRandom);
        }
    }

}