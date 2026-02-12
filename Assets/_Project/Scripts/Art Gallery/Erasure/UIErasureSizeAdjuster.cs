using UnityEngine;
using UnityEngine.UI;

public class UIErasureSizeAdjuster : MonoBehaviour
{
    [SerializeField] private Slider sizeSlider;
    private void Start()
    {
        sizeSlider.minValue = ErasureManager.Instance.MinSize;
        sizeSlider.maxValue = ErasureManager.Instance.MaxSize;
        sizeSlider.value = ErasureManager.Instance.CurrentSize;
    }
    private void OnEnable() => sizeSlider.onValueChanged.AddListener(OnSizeChanged);
    private void OnDisable() => sizeSlider.onValueChanged.RemoveListener(OnSizeChanged);
    public void OnSizeChanged(float value) => ErasureManager.Instance.SetSize(value);
}
