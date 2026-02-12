using UnityEngine;
using UnityEngine.UI;

public class UIBrushOpacityAdjuster : MonoBehaviour
{
    [SerializeField] private Slider opacityBrushSlider;

    private void Start()
    {
        opacityBrushSlider.minValue = 0;
        opacityBrushSlider.maxValue = 1;
        opacityBrushSlider.value = BrushManager.Instance.CurrentOpacity;
    }
    private void OnEnable() => opacityBrushSlider.onValueChanged.AddListener(OnOpacityChanged);
    private void OnDisable() => opacityBrushSlider.onValueChanged.RemoveListener(OnOpacityChanged);
    public void OnOpacityChanged(float value) => BrushManager.Instance.SetOpacity(value);
}
