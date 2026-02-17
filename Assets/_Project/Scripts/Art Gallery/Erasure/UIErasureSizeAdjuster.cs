using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIErasureSizeAdjuster : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Slider sizeSlider;

    [Header("Previewer")]
    [SerializeField] private GameObject sizePreviewContainer;
    [SerializeField] private Image sizePreviewImage;
    [SerializeField] private TextMeshProUGUI sizePreviewText;
    [SerializeField, Range(0f, 6f)] private float maxPreviewScale = 4f;

    private void Start()
    {
        sizePreviewContainer.SetActive(false);

        sizeSlider.minValue = ErasureManager.Instance.MinSize;
        sizeSlider.maxValue = ErasureManager.Instance.MaxSize;
        sizeSlider.value = ErasureManager.Instance.CurrentSize;

        sizePreviewImage.transform.localScale = Vector3.one * Mathf.Lerp(0f, maxPreviewScale, sizeSlider.value / ErasureManager.Instance.MaxSize);
    }
    private void OnEnable()
    {
        sizeSlider.onValueChanged.AddListener(OnSizeChanged);
        ErasureManager.OnSizeChanged += UpdateSlider;
    }

    private void OnDisable()
    {
        sizeSlider.onValueChanged.RemoveListener(OnSizeChanged);
        ErasureManager.OnSizeChanged -= UpdateSlider;
    }

    public void OnSizeChanged(float value)
    {
        ErasureManager.Instance.SetSize(value);

        UpdatePreview(value);
    }

    private void UpdatePreview(float value)
    {
        float t = value / BrushManager.Instance.MaxSize;
        sizePreviewImage.transform.localScale = Vector3.one * Mathf.Lerp(0f, maxPreviewScale, t);
        sizePreviewText.text = $"<b>Size</b> {t * 100:F0}%";
    }

    private void UpdateSlider(float value)
    {
        sizeSlider.value = value;
        UpdatePreview(value);
    }
    public void OnPointerDown(PointerEventData eventData) => sizePreviewContainer.SetActive(true);
    public void OnPointerUp(PointerEventData eventData) => sizePreviewContainer.SetActive(false);
}
