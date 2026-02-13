using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIBrushOpacityAdjuster : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Slider opacityBrushSlider;

    [Header("Previewer")]
    [SerializeField] private GameObject sizePreviewContainer;
    [SerializeField] private Image sizePreviewImage;
    [SerializeField] private TextMeshProUGUI sizePreviewText;

    private void Start()
    {
        sizePreviewContainer.SetActive(false);

        opacityBrushSlider.minValue = 0;
        opacityBrushSlider.maxValue = 1;
        opacityBrushSlider.value = BrushManager.Instance.CurrentOpacity;
    }
    private void OnEnable()
    {
        opacityBrushSlider.onValueChanged.AddListener(OnOpacityChanged);
        BrushManager.OnBrushChanged += OnBrushChanged;
    }

    private void OnDisable()
    {
        opacityBrushSlider.onValueChanged.RemoveListener(OnOpacityChanged);
        BrushManager.OnBrushChanged -= OnBrushChanged;
    }

    public void OnOpacityChanged(float value)
    {
        BrushManager.Instance.SetOpacity(value);

        float t = value / 1f;
        Color color = sizePreviewImage.color;
        color.a = Mathf.Lerp(0f, 1f, t);
        sizePreviewImage.color = color;
        sizePreviewText.text = $"<b>Opacity</b> {t * 100:F0}%";
    }

    private void OnBrushChanged(BrushData brushData) => sizePreviewImage.sprite = brushData.BrushSprite;

    public void OnPointerDown(PointerEventData eventData) => sizePreviewContainer.SetActive(true);
    public void OnPointerUp(PointerEventData eventData) => sizePreviewContainer.SetActive(false);
}
