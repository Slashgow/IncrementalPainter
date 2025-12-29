using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class AutoClickerInput : MonoBehaviour
{
    [SerializeField] private InputActionReference toggleAutoclickerInputActionReference;

    public event Action OnToggleAutoclicker;

    private void OnEnable()
    {
        toggleAutoclickerInputActionReference.asset.Enable();

        toggleAutoclickerInputActionReference.action.performed += ToggleAutoclickerInputAction_performed;
    }

    private void OnDisable()
    {
        toggleAutoclickerInputActionReference.action.performed -= ToggleAutoclickerInputAction_performed;

        toggleAutoclickerInputActionReference.asset.Disable();
    }

    private void ToggleAutoclickerInputAction_performed(InputAction.CallbackContext context)
    {
        OnToggleAutoclicker?.Invoke();
    }
}
