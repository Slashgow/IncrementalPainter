using System;
using UnityEngine;

public class NukeItem : MonoBehaviour
{
    [SerializeField] private SimpleVacuumable vacuumable;

    public static event Action<Vector3> OnNukeCollectedEvent;

    private void OnEnable() => vacuumable.OnVacuumCollected += OnNukeCollected;
    private void OnDisable() => vacuumable.OnVacuumCollected -= OnNukeCollected;

    private void OnNukeCollected(IVacuumer vacuumer)
    {
        OnNukeCollectedEvent?.Invoke(this.transform.position);

        if (gameObject != null)
            Destroy(gameObject);
    }
}
