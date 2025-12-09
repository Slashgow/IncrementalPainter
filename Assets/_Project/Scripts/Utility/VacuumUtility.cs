using UnityEngine;

public static class VacuumUtility
{
    public static bool IsValidVacuumable(IVacuumable vacuumable)
    {
        if (vacuumable == null)
            return false;

        if (vacuumable.Transform == null)
            return false;

        if (vacuumable is MonoBehaviour mono && mono == null)
            return false;

        return true;
    }

    public static Vector3 CalculateVacuumMovement(Vector3 currentPosition,Vector3 vacuumCenter,float vacuumForce,float resistance,float deltaTime)
    {
        Vector3 direction = vacuumCenter - currentPosition;
        float distance = direction.magnitude;

        if (distance < 0.01f)
            return Vector3.zero;

        direction.Normalize();
        float effectiveForce = vacuumForce * (1f - resistance);

        return direction * effectiveForce * deltaTime;
    }

    public static bool IsWithinCollectionDistance(Vector3 objectPosition, Vector3 vacuumCenter, float collectDistance)
    {
        return Vector3.Distance(objectPosition, vacuumCenter) <= collectDistance;
    }
}