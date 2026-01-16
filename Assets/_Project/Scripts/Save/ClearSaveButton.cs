using System;
using UnityEngine;
using UnityEngine.UI;

public class ClearSaveButton : MonoBehaviour
{
    [SerializeField] ClearSavePopup popup;
    [SerializeField] private Button clearSaveButton;

    private void OnEnable()
    {
        HideClearSavePopup();
        clearSaveButton.onClick.AddListener(ShowClearSavePopup);
    }

    private void OnDisable() => clearSaveButton.onClick.RemoveListener(ShowClearSavePopup);

    private void ShowClearSavePopup() => popup.gameObject.SetActive(true);
    private void HideClearSavePopup() => popup.gameObject.SetActive(false);
}
