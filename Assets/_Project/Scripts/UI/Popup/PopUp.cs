using System;
using UnityEngine;
using UnityEngine.UI;
public class PopUp : MonoBehaviour
{
    [SerializeField] private Button doActionButton, cancelButton;

    public Action OnClosed;

    protected virtual void Awake() => gameObject.SetActive(false);

    protected virtual void OnEnable()
    {
        doActionButton.onClick.AddListener(OnClickDoActionButton);
        cancelButton.onClick.AddListener(OnClickCancelButton);
    }

    private void OnDisable()
    {
        doActionButton.onClick.RemoveListener(OnClickDoActionButton);
        cancelButton.onClick.RemoveListener(OnClickCancelButton);
    }

    protected virtual void OnClickDoActionButton()
    {
        gameObject.SetActive(false);
        Close();
    }

    protected virtual void OnClickCancelButton()
    {
        gameObject.SetActive(false);
        Close();
    }

    protected void Close()
    {
        OnClosed?.Invoke();
        OnClosed = null;
    }
}
