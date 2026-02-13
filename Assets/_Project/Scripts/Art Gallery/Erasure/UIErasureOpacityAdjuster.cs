using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIErasureOpacityAdjuster : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Slider opacitySlider;

    [Header("Previewer")]
    [SerializeField] private GameObject previewContainer;
    [SerializeField] private Image previewImage;
    [SerializeField] private TextMeshProUGUI previewText;

    private void Start()
    {
        previewContainer.SetActive(false);

        opacitySlider.minValue = 0f;
        opacitySlider.maxValue = 1f;
        opacitySlider.value = ErasureManager.Instance.CurrentOpacity;
    }
    private void OnEnable()
    {
        opacitySlider.onValueChanged.AddListener(OnOpacityChanged);
    }

    private void OnDisable()
    {
        opacitySlider.onValueChanged.RemoveListener(OnOpacityChanged);
    }

    public void OnOpacityChanged(float value)
    {
        ErasureManager.Instance.SetOpacity(value);

        float t = value / 1f;
        Color color = previewImage.color;
        color.a = Mathf.Lerp(0f, 1f, t);
        previewImage.color = color;
        previewText.text = $"<b>Opacity</b> {t * 100:F0}%";
    }

    public void OnPointerDown(PointerEventData eventData) => previewContainer.SetActive(true);
    public void OnPointerUp(PointerEventData eventData) => previewContainer.SetActive(false);
}