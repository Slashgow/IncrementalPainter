using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public class UnlockTamponPopup : PopUp
{
    [SerializeField] private TextMeshProUGUI themeNameText;
    [SerializeField] private Image previewImage;
    [SerializeField] private LocalizedString unlockLocalizedString;

    private Canvas parentCanvas;
    private TamponData tamponData;

    public void Initialize(TamponData tamponData, Canvas parent)
    {
        this.tamponData = tamponData;
        parentCanvas = parent;


#if !UNITY_WEBGL
        themeNameText.text = $"{tamponData.ItemName} {unlockLocalizedString.GetLocalizedString()}";
#endif

#if UNITY_WEBGL

        unlockLocalizedString.GetLocalizedStringAsync().Completed += (handle) =>
        {
            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                     themeNameText.text = $"{tamponData.ItemName} {handle.Result}";
            }
        };
#endif

        if (tamponData != null)
        {
            previewImage.sprite = tamponData.TamponSprite;
            Debug.Log($"unlocked tampon: {tamponData.ItemName}");
        }

        parentCanvas.gameObject.SetActive(true);
        gameObject.SetActive(true);

        if (GameManager.HasInstance)
            GameManager.Instance.Pause();
    }

    protected override void OnClickDoActionButton()
    {
        base.OnClickDoActionButton();

        gameObject.SetActive(false);
        parentCanvas.gameObject.SetActive(false);

        if (GameManager.HasInstance)
            GameManager.Instance.Resume();
    }


    protected override void OnClickCancelButton()
    {
        base.OnClickCancelButton();
        parentCanvas.gameObject.SetActive(false);

        if (GameManager.HasInstance)
            GameManager.Instance.Resume();
    }
}
