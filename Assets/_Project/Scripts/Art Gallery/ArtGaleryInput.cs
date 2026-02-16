using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ArtGaleryInput : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActionAsset;
    [SerializeField] private InputActionReference rotateInputActionReference;
    [SerializeField] private InputActionReference undoInputActionReference;
    [SerializeField] private InputActionReference redoInputActionReference;

    private float rotateInput;
    public float RotateInput => rotateInput;
    public static event Action OnUndoPerformed;
    public static event Action OnRedoPerformed;

    private void OnEnable()
    {
        inputActionAsset.Enable();

        rotateInputActionReference.action.performed += RotatePeformed;
        rotateInputActionReference.action.canceled += RotatePeformed;
        undoInputActionReference.action.performed += UndoPerformed;
        redoInputActionReference.action.performed += RedoPerformed;
    }

    private void OnDisable()
    {
        rotateInputActionReference.action.performed -= RotatePeformed;
        rotateInputActionReference.action.canceled -= RotatePeformed;
        undoInputActionReference.action.performed -= UndoPerformed;
        redoInputActionReference.action.performed -= RedoPerformed;

        inputActionAsset.Disable();
    }

    private void RotatePeformed(InputAction.CallbackContext context) => rotateInput = context.ReadValue<Vector2>().y;
    private void RedoPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("Redo input performed");
        OnRedoPerformed?.Invoke();
    }

    private void UndoPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("Undo input performed");
        OnUndoPerformed?.Invoke();
    }
}
