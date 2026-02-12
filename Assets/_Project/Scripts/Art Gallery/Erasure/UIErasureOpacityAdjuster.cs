using UnityEngine;
using UnityEngine.UI;

public class UIErasureOpacityAdjuster : MonoBehaviour
{
    [SerializeField] private Slider opacitySlider;
    private void Start()
    {
        opacitySlider.minValue = 0f;
        opacitySlider.maxValue = 1f;
        opacitySlider.value = ErasureManager.Instance.CurrentOpacity;
    }
    private void OnEnable() => opacitySlider.onValueChanged.AddListener(OnOpacityChanged);
    private void OnDisable() => opacitySlider.onValueChanged.RemoveListener(OnOpacityChanged);
    public void OnOpacityChanged(float value) => ErasureManager.Instance.SetOpacity(value);
}