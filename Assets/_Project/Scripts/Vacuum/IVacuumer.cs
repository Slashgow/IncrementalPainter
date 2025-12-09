using UnityEngine;

public interface IVacuumer
{
    Transform Transform { get; }
    float VacuumForce { get; }
    float VacuumRange { get; }
    float CollectDistance { get; }
    bool IsActive { get; }

    void CollectVacuumable(IVacuumable vacuumable);
}