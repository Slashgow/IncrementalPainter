using UnityEngine;
using UnityEngine.UI;

public class UIUndo : MonoBehaviour
{
    [SerializeField] private Button undoButton;

    private void OnEnable()
    {
        undoButton.onClick.AddListener(OnUndoButtonClicked);
        CommandHistory.OnExecutedCommand += UpdateUndoButtonState;
    }

    private void OnDisable()
    {
        undoButton.onClick.RemoveListener(OnUndoButtonClicked);
        CommandHistory.OnExecutedCommand -= UpdateUndoButtonState;
    }

    private void UpdateUndoButtonState()
    {
        if (CommandHistory.Instance.CanUndo && !undoButton.interactable)
        {
            undoButton.interactable = true;
        }
        else if(!CommandHistory.Instance.CanUndo && undoButton.interactable)
        {
            undoButton.interactable = false;
        }
    }

    private void OnUndoButtonClicked()
    {
        CommandHistory.Instance.Undo();
    }
}
