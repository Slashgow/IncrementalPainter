using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ArtGaleryInput : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActionAsset;

    [Header("Command")]
    [SerializeField] private InputActionReference saveInputActionReference;
    [SerializeField] private InputActionReference undoInputActionReference;
    [SerializeField] private InputActionReference redoInputActionReference;

    [Header("Tool Selection")]
    [SerializeField] private InputActionReference selectBrushInputActionReference;
    [SerializeField] private InputActionReference selectEraserInputActionReference;
    [SerializeField] private InputActionReference selectStickerInputActionReference;
    [SerializeField] private InputActionReference selectPaintingInputActionReference;
    [SerializeField] private InputActionReference selectColorInputActionReference;

    [Header("Tool Specific")]
    [SerializeField] private InputActionReference rotateInputActionReference;
    [SerializeField, Range(0f,5f)] private float sizeSensitivity = 1f;
    [SerializeField] private InputActionReference changeSizeBrushInputActionReference;
    [SerializeField, Range(0f,1f)] private float opacitySensitivity = 0.1f;
    [SerializeField] private InputActionReference changeOpacityInputActionReference;

    private float rotateInput;
    public float RotateInput => rotateInput;

    public static event Action OnUndoPerformed;
    public static event Action OnRedoPerformed;
    public static event Action OnSavePerformed;
    public static event Action<ToolType> OnToolSelected;
    public static event Action<float> OnBrushSizeChanged;
    public static event Action<float> OnBrushOpacityChanged;

    private void OnEnable()
    {
        inputActionAsset.Enable();

        undoInputActionReference.action.performed += UndoPerformed;
        redoInputActionReference.action.performed += RedoPerformed;
        saveInputActionReference.action.performed += SavePerformed;

        selectBrushInputActionReference.action.performed += SelectBrushPerformed;
        selectEraserInputActionReference.action.performed += SelectEraserPerfomed;
        selectStickerInputActionReference.action.performed += SelectStickerPerformed;
        selectPaintingInputActionReference.action.performed += SelectPaintingPerformed;
        selectColorInputActionReference.action.performed += SelectColorPerformed;

        changeSizeBrushInputActionReference.action.performed += ChangeSizeBrush;
        changeOpacityInputActionReference.action.performed += ChangeOpacity;

        rotateInputActionReference.action.performed += RotatePeformed;
        rotateInputActionReference.action.canceled += RotatePeformed;
    }

    private void OnDisable()
    {
        undoInputActionReference.action.performed -= UndoPerformed;
        redoInputActionReference.action.performed -= RedoPerformed;
        saveInputActionReference.action.performed -= SavePerformed;

        selectBrushInputActionReference.action.performed -= SelectBrushPerformed;
        selectEraserInputActionReference.action.performed -= SelectEraserPerfomed;
        selectStickerInputActionReference.action.performed -= SelectStickerPerformed;
        selectPaintingInputActionReference.action.performed -= SelectPaintingPerformed;
        selectColorInputActionReference.action.performed -= SelectColorPerformed;

        changeSizeBrushInputActionReference.action.performed -= ChangeSizeBrush;
        changeOpacityInputActionReference.action.performed -= ChangeOpacity;

        rotateInputActionReference.action.performed -= RotatePeformed;
        rotateInputActionReference.action.canceled -= RotatePeformed;

        inputActionAsset.Disable();
    }

    private void RotatePeformed(InputAction.CallbackContext context) => rotateInput = context.ReadValue<Vector2>().y;
    private void RedoPerformed(InputAction.CallbackContext context) => OnRedoPerformed?.Invoke();
    private void UndoPerformed(InputAction.CallbackContext context) => OnUndoPerformed?.Invoke();
    private void SavePerformed(InputAction.CallbackContext context) => OnSavePerformed?.Invoke();
    private void SelectBrushPerformed(InputAction.CallbackContext context) => OnToolSelected?.Invoke(ToolType.Brush);
    private void SelectEraserPerfomed(InputAction.CallbackContext context) => OnToolSelected?.Invoke(ToolType.Eraser);
    private void SelectStickerPerformed(InputAction.CallbackContext context) => OnToolSelected?.Invoke(ToolType.Tampon);
    private void SelectPaintingPerformed(InputAction.CallbackContext context) => OnToolSelected?.Invoke(ToolType.Painting);
    private void SelectColorPerformed(InputAction.CallbackContext context) => OnToolSelected?.Invoke(ToolType.ColorSelector);
    private void ChangeSizeBrush(InputAction.CallbackContext context) => OnBrushSizeChanged?.Invoke(context.ReadValue<Vector2>().y * sizeSensitivity);
    private void ChangeOpacity(InputAction.CallbackContext context) => OnBrushOpacityChanged?.Invoke(context.ReadValue<Vector2>().y * opacitySensitivity);
}
