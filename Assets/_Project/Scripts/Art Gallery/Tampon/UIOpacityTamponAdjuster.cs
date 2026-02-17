using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIOpacityTamponAdjuster : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Slider opacitySlider;

    [Header("Previewer")]
    [SerializeField] private GameObject sizePreviewContainer;
    [SerializeField] private Image sizePreviewImage;
    [SerializeField] private TextMeshProUGUI sizePreviewText;

    private void Start()
    {
        sizePreviewContainer.SetActive(false);

        opacitySlider.minValue = 0;
        opacitySlider.maxValue = 1;
        opacitySlider.value = TamponManager.Instance.CurrentOpacity;
    }
    private void OnEnable()
    {
        opacitySlider.onValueChanged.AddListener(OnOpacityChanged);
        TamponManager.OnTamponChanged += OnTamponChanged;
        TamponManager.OnTamponOpacityChanged += UpdateSlider;
    }

    private void OnDisable()
    {
        opacitySlider.onValueChanged.RemoveListener(OnOpacityChanged);
        TamponManager.OnTamponChanged -= OnTamponChanged;
        TamponManager.OnTamponOpacityChanged -= UpdateSlider;
    }

    public void OnOpacityChanged(float value)
    {
        TamponManager.Instance.SetOpacity(value);

        UpdatePreview(value);
    }

    private void UpdatePreview(float value)
    {
        float t = value / 1f;
        Color color = sizePreviewImage.color;
        color.a = Mathf.Lerp(0f, 1f, t);
        sizePreviewImage.color = color;
        sizePreviewText.text = $"<b>Opacity</b> {t * 100:F0}%";
    }

    private void UpdateSlider(float value)
    {
        opacitySlider.value = value;
        UpdatePreview(value);
    }
    private void OnTamponChanged(TamponData tamponData) => sizePreviewImage.sprite = tamponData.TamponSprite;

    public void OnPointerDown(PointerEventData eventData) => sizePreviewContainer.SetActive(true);
    public void OnPointerUp(PointerEventData eventData) => sizePreviewContainer.SetActive(false);
}
