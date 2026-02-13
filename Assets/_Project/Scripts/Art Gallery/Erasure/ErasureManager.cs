using inkolorgames;
using PaintIn2D;
using UnityEngine;

public class ErasureManager : PersistentMonoSingleton<ErasureManager>
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

    protected override void Awake()
    {
        base.Awake();
        SetSize(defaultSize, false);
        DisableEraser();
    }

    public void EnableEraser() => paintDecal2D.gameObject.SetActive(true);
    public void DisableEraser() => paintDecal2D.gameObject.SetActive(false);

    public void SetSize(float size, bool notifyListeners = true)
    {
        currentSize = Mathf.Clamp(size, minSize, maxSize);
        paintDecal2D.Scale = Vector3.one * currentSize;
    }


    public void SetOpacity(float opacity, bool notifyListeners = true)
    {
        currentOpacity = Mathf.Clamp01(opacity);
        paintDecal2D.Opacity = opacity;
    }

}
