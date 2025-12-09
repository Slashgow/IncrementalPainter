using System;
using UnityEngine;

public interface IVacuumable
{
    Transform Transform { get; }
    bool CanBeVacuumed { get; }
    float VacuumResistance { get; } // 0 = no resistance, 1 = full resistance

    void Initialize(Vector3 targetPosition);
    void ApplyVacuumMovement(Vector3 movement);
    void OnVacuumedCollect(IVacuumer vacuumer);

    event Action<IVacuumer> OnVacuumCollected;
}
