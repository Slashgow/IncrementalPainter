using UnityEngine;
public class MagnetItem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SimpleVacuumable vacuumable;

    [Header("Magnet Settings")]
    [SerializeField] private float magnetForceBoost = 10f;
    [SerializeField] private float magnetDuration = 2f;
    [SerializeField] private float magnetRangeBoost = 5f;

    private bool isActivated = false;

    private void OnEnable() => vacuumable.OnVacuumCollected += OnMagnetCollected;
    private void OnDisable() => vacuumable.OnVacuumCollected -= OnMagnetCollected;

    private void OnMagnetCollected(IVacuumer vacuumer)
    {
        if (isActivated)
            return;

        isActivated = true;

        if (vacuumer is Vacuumer vacuumerComponent)
        {
            vacuumerComponent.AddTemporaryBoost(magnetForceBoost, magnetRangeBoost, magnetDuration);

            if (gameObject != null)
                Destroy(gameObject);
        }
    }
}