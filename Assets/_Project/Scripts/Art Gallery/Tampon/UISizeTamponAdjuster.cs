using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISizeTamponAdjuster : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
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

        sizeSlider.minValue = TamponManager.Instance.MinSize;
        sizeSlider.maxValue = TamponManager.Instance.MaxSize;
        sizeSlider.value = TamponManager.Instance.CurrentSize;

        sizePreviewImage.transform.localScale = Vector3.one * Mathf.Lerp(0f, maxPreviewScale, sizeSlider.value / TamponManager.Instance.MaxSize);

        OnTamponChanged(TamponManager.Instance.CurrentTampon);
    }
    private void OnEnable()
    {
        sizeSlider.onValueChanged.AddListener(OnSizeChanged);
        TamponManager.OnTamponChanged += OnTamponChanged;
        TamponManager.OnTamponSizeChanged += UpdateSlider;
    }

    private void OnDisable()
    {
        sizeSlider.onValueChanged.RemoveListener(OnSizeChanged);
        TamponManager.OnTamponChanged -= OnTamponChanged;
        TamponManager.OnTamponSizeChanged -= UpdateSlider;
    }

    public void OnSizeChanged(float value)
    {
        TamponManager.Instance.SetSize(value);

        UpdatePreview(value);
    }

    private void UpdatePreview(float value)
    {
        float t = value / TamponManager.Instance.MaxSize;
        sizePreviewImage.transform.localScale = Vector3.one * Mathf.Lerp(0f, maxPreviewScale, t);
        sizePreviewText.text = $"<b>Size</b> {t * 100:F0}%";
    }

    private void UpdateSlider(float value)
    {
        sizeSlider.value = value;
        UpdatePreview(value);
    }
    private void OnTamponChanged(TamponData tamponData) => sizePreviewImage.sprite = tamponData.TamponSprite;

    public void OnPointerDown(PointerEventData eventData)
    {
        sizePreviewContainer.SetActive(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        sizePreviewContainer.SetActive(false);
    }
}
