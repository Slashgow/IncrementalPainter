using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIBrushSizeAdjuster : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Slider sizeBrushSlider;

    [Header("Previewer")]
    [SerializeField] private GameObject sizePreviewContainer;
    [SerializeField] private Image sizePreviewImage;
    [SerializeField] private TextMeshProUGUI sizePreviewText;
    [SerializeField, Range(0f, 6f)] private float maxPreviewScale = 4f;

    private void Start()
    {
        sizePreviewContainer.SetActive(false);

        sizeBrushSlider.minValue = BrushManager.Instance.MinSize;
        sizeBrushSlider.maxValue = BrushManager.Instance.MaxSize;
        sizeBrushSlider.value = BrushManager.Instance.CurrentSize;

        sizePreviewImage.transform.localScale = Vector3.one * Mathf.Lerp(0f, maxPreviewScale, sizeBrushSlider.value / BrushManager.Instance.MaxSize);

        OnBrushChanged(BrushManager.Instance.CurrentBrush);
    }
    private void OnEnable()
    {
        sizeBrushSlider.onValueChanged.AddListener(OnSizeChanged);
        BrushManager.OnBrushChanged += OnBrushChanged;
    }

    private void OnDisable()
    {
        sizeBrushSlider.onValueChanged.RemoveListener(OnSizeChanged);
        BrushManager.OnBrushChanged -= OnBrushChanged;
    }

    public void OnSizeChanged(float value)
    {
        BrushManager.Instance.SetSize(value);

        float t = value / BrushManager.Instance.MaxSize;
        sizePreviewImage.transform.localScale = Vector3.one * Mathf.Lerp(0f, maxPreviewScale, t);
        sizePreviewText.text = $"<b>Size</b> {t * 100:F0}%";

    }

    private void OnBrushChanged(BrushData brushData) => sizePreviewImage.sprite = brushData.BrushSprite;

    public void OnPointerDown(PointerEventData eventData) => sizePreviewContainer.SetActive(true);
    public void OnPointerUp(PointerEventData eventData) => sizePreviewContainer.SetActive(false);
}
