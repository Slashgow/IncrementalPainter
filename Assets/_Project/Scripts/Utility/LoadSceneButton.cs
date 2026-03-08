using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class LoadSceneButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField, Range(0, 3)] private int sceneIndex;

    [SerializeField] private bool showWarning = false;
    [SerializeField, ShowIf("showWarning")] private PopUp warningSavePopUp;

    private void OnEnable() => button.onClick.AddListener(OnButtonClicked);
    private void OnDisable() => button.onClick.RemoveListener(OnButtonClicked);
    private void OnButtonClicked()
    {
        if(!showWarning)
            SceneLoader.Instance.LoadSceneAsync(sceneIndex);
        else
            warningSavePopUp.gameObject.SetActive(true);
    }
}