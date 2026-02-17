using UnityEngine;

public class ToolSelectionManager : MonoBehaviour
{
    [SerializeField] private ToolButton[] toolButtons;

    private ToolStateMachine toolStateMachine;
    private ToolButton currentlySelectedButton;

    private void OnEnable() => ArtGaleryInput.OnToolSelected += OnToolSelected;
    private void OnDisable() => ArtGaleryInput.OnToolSelected -= OnToolSelected;

    private void Awake()
    {
        toolStateMachine = new ToolStateMachine();
 
        foreach (var button in toolButtons)
        {
            button.OnSelectEvent += OnToolSelected;
        }

        toolStateMachine.OnToolChanged += OnToolChangedInStateMachine;

        UpdateButtonVisuals(ToolStateMachine.CurrentState.ToolType);
    }

    private void OnDestroy()
    {
        foreach (var button in toolButtons)
        {
            button.OnSelectEvent -= OnToolSelected;
        }

        toolStateMachine.OnToolChanged -= OnToolChangedInStateMachine;
    }

    private void Update()
    {
        toolStateMachine.Update();
    }

    private void OnToolSelected(ToolType selectedTool)
    {
        toolStateMachine.TransitionToTool(selectedTool);
    }

    private void OnToolChangedInStateMachine(ToolType newTool)
    {
        // Update UI to reflect current selection
        UpdateButtonVisuals(newTool);
    }

    private void UpdateButtonVisuals(ToolType selectedTool)
    {
        foreach (var button in toolButtons)
        {
            bool isSelected = button.GetSelectableData() == selectedTool;
            button.SetSelected(isSelected);
        }
    }

    public ToolType GetCurrentTool() => ToolStateMachine.CurrentState.ToolType;
    public void SelectTool(ToolType tool) => toolStateMachine.TransitionToTool(tool);
}