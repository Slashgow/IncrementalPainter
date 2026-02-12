using System;
using UnityEngine;
using UnityEngine.UI;

public class ToolButton : MonoBehaviour, IUISelectable<ToolType>
{
    [SerializeField] private ToolType toolType;
    [SerializeField] private GameObject selectedIndicator;
    [SerializeField] private Button button;

    public event Action<ToolType> OnSelectEvent;
    private void Awake()
    {
        selectedIndicator.SetActive(false);
    }

    private void OnEnable() => button.onClick.AddListener(OnButtonClicked);
    private void OnDisable() => button.onClick.RemoveListener(OnButtonClicked);

    public void OnSelect(ToolType data)
    {
        OnSelectEvent?.Invoke(data);
    }

    public ToolType GetSelectableData()
    {
        return toolType;
    }

    public void OnButtonClicked()
    {
        OnSelect(toolType);
    }

    public void SetSelected(bool selected)
    {
        if (selectedIndicator != null)
            selectedIndicator.SetActive(selected);
    }
}
