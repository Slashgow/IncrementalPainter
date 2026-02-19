using UnityEngine;
using UnityEngine.Events;

public class ShieldableListener : MonoBehaviour
{
    [SerializeField] private UnityEvent<Vector3, Color> onAnyShieldBroken;
    private void OnEnable() => SimpleShieldable.OnAnyShieldBroken += HandleAnyShieldBroken;
    private void OnDisable() => SimpleShieldable.OnAnyShieldBroken -= HandleAnyShieldBroken;

    private void HandleAnyShieldBroken(Vector3 worldPosition, Color color, float scale, bool tryPaint) => onAnyShieldBroken?.Invoke(worldPosition, color);
}
