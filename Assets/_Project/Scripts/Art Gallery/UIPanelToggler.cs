using UnityEngine;
using UnityEngine.UI;

public class UIPanelToggler : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private GameObject panel;

    //private void Awake() => panel.SetActive(false);
    private void OnEnable() => button.onClick.AddListener(TogglePanel);
    private void OnDisable() => button.onClick.RemoveListener(TogglePanel);

    private void TogglePanel()
    {
        panel.SetActive(!panel.activeSelf);
    }
}
