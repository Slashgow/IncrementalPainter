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

    public void Initialize(TamponData tamponData, Canvas parent)
    {
        parentCanvas = parent;

#if !UNITY_WEBGL
        themeNameText.text = $"{tamponData.ItemName} {unlockLocalizedString.GetLocalizedString()}";
#else
        unlockLocalizedString.GetLocalizedStringAsync().Completed += (handle) =>
        {
            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                themeNameText.text = $"{tamponData.ItemName} {handle.Result}";
        };
#endif

        if (tamponData != null)
            previewImage.sprite = tamponData.TamponSprite;

        parentCanvas.gameObject.SetActive(true);
        gameObject.SetActive(true);

        if (GameManager.HasInstance)
            GameManager.Instance.Pause();
    }

    protected override void OnClickDoActionButton()
    {
        Dismiss();
    }

    protected override void OnClickCancelButton()
    {
        Dismiss();
    }

    private void Dismiss()
    {
        gameObject.SetActive(false);

        // Only hide the canvas if no more sibling popups are active
        if (parentCanvas != null && !HasActivePopupSibling())
            parentCanvas.gameObject.SetActive(false);

        if (GameManager.HasInstance)
            GameManager.Instance.Resume();

        Close(); // notifies the spawner queue
    }

    private bool HasActivePopupSibling()
    {
        foreach (Transform child in parentCanvas.transform)
            if (child.gameObject != gameObject && child.gameObject.activeSelf && child.GetComponent<PopUp>() != null)
                return true;
        return false;
    }
}