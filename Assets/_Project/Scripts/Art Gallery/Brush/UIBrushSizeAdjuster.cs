using UnityEngine;
using UnityEngine.UI;

public class UIBrushSizeAdjuster : MonoBehaviour
{
    [SerializeField] private Slider sizeBrushSlider;

    private void Start()
    {
        sizeBrushSlider.minValue = BrushManager.Instance.MinSize;
        sizeBrushSlider.maxValue = BrushManager.Instance.MaxSize;
        sizeBrushSlider.value = BrushManager.Instance.CurrentSize;
    }
    private void OnEnable() => sizeBrushSlider.onValueChanged.AddListener(OnSizeChanged);
    private void OnDisable() => sizeBrushSlider.onValueChanged.RemoveListener(OnSizeChanged);
    public void OnSizeChanged(float value) => BrushManager.Instance.SetSize(value);
}
