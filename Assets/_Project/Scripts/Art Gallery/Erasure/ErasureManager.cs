using System;
using inkolorgames;
using PaintIn2D;
using UnityEngine;

public class ErasureManager : MonoSingleton<ErasureManager>
{
    [SerializeField] private CwPaintDecal2D paintDecal2D;

    [SerializeField, Range(0f, 100f)] private float defaultSize;
    [SerializeField, Range(0f, 100f)] private float minSize;
    [SerializeField, Range(0f, 100f)] private float maxSize;

    private float currentSize = 10f;
    private float currentOpacity = 1f;

    public float MinSize => minSize;
    public float MaxSize => maxSize;
    public float CurrentSize => currentSize;
    public float CurrentOpacity => currentOpacity;

    public static event Action<float> OnSizeChanged;
    public static event Action<float> OnOpacityChanged;

    protected override void Awake()
    {
        base.Awake();
        SetSize(defaultSize, false);
        DisableEraser();
    }

    private void OnEnable()
    {
        ArtGaleryInput.OnBrushSizeChanged += ChangeSize;
        ArtGaleryInput.OnBrushOpacityChanged += ChangeOpacity;
    }
    private void OnDisable()
    {
        ArtGaleryInput.OnBrushSizeChanged -= ChangeSize;
        ArtGaleryInput.OnBrushOpacityChanged -= ChangeOpacity;
    }

    public void EnableEraser() => paintDecal2D.gameObject.SetActive(true);
    public void DisableEraser() => paintDecal2D.gameObject.SetActive(false);

    private void ChangeSize(float delta)
    {
        if(ToolStateMachine.CurrentState.ToolType != ToolType.Eraser)
            return;

        SetSize(currentSize + delta, true);
    }

    public void SetSize(float size, bool notifyListeners = false)
    {
        currentSize = Mathf.Clamp(size, minSize, maxSize);
        paintDecal2D.Scale = Vector3.one * currentSize;

        if (notifyListeners)
            OnSizeChanged?.Invoke(currentSize);
    }

    private void ChangeOpacity(float delta)
    {
        if (ToolStateMachine.CurrentState.ToolType != ToolType.Eraser)
            return;

        SetOpacity(currentOpacity + delta, true);
    }

    public void SetOpacity(float opacity, bool notifyListeners = false)
    {
        currentOpacity = Mathf.Clamp01(opacity);
        paintDecal2D.Opacity = opacity;

        if (notifyListeners)
            OnOpacityChanged?.Invoke(currentOpacity);
    }

}
