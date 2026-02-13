using UnityEngine;
using UnityEngine.InputSystem;

public class ArtGaleryInput : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActionAsset;
    [SerializeField] private InputActionReference rotateInputActionReference;

    private float rotateInput;
    public float RotateInput => rotateInput;

    private void OnEnable()
    {
        inputActionAsset.Enable();

        rotateInputActionReference.action.performed += RotatePeformed;
        rotateInputActionReference.action.canceled += RotatePeformed;
    }

    private void OnDisable()
    {
        rotateInputActionReference.action.performed -= RotatePeformed;
        rotateInputActionReference.action.canceled -= RotatePeformed;

        inputActionAsset.Disable();
    }

    private void RotatePeformed(InputAction.CallbackContext context)
    {
        rotateInput = context.ReadValue<Vector2>().y;
    }
}
