using UnityEngine;
using UnityEngine.UI;

public class UIRedo : MonoBehaviour
{
    [SerializeField] private Button redoButton;
    private void OnEnable()
    {
        redoButton.onClick.AddListener(OnRedoButtonClicked);
        CommandHistory.OnExecutedCommand += UpdateRedoButtonState;
    }
    private void OnDisable()
    {
        redoButton.onClick.RemoveListener(OnRedoButtonClicked);
        CommandHistory.OnExecutedCommand -= UpdateRedoButtonState;
    }
    private void UpdateRedoButtonState()
    {
        if (CommandHistory.Instance.CanRedo && !redoButton.interactable)
        {
            redoButton.interactable = true;
        }
        else if(!CommandHistory.Instance.CanRedo && redoButton.interactable)
        {
            redoButton.interactable = false;
        }
    }
    private void OnRedoButtonClicked()
    {
        CommandHistory.Instance.Redo();
    }
}
