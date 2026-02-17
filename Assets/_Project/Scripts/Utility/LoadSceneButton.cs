using System;
using UnityEngine;
using UnityEngine.UI;

public class LoadSceneButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField, Range(0, 3)] private int sceneIndex;

    private void OnEnable() => button.onClick.AddListener(OnButtonClicked);
    private void OnDisable() => button.onClick.RemoveListener(OnButtonClicked);
    private void OnButtonClicked() => SceneLoader.Instance.LoadSceneAsync(sceneIndex);
}