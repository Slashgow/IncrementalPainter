using UnityEngine;

public static class MagnetUtility
{
    public static Vector3 CalculatePullMovement(Vector3 currentPosition,Vector3 targetPosition,float attractionForce,float resistance,float deltaTime)
    {
        Vector3 direction = targetPosition - currentPosition;
        float distance = direction.magnitude;

        if (distance < 0.01f)
            return Vector3.zero;

        direction.Normalize();
        float effectiveForce = attractionForce * (1f - resistance);

        return direction * effectiveForce * deltaTime;
    }

    public static Vector3 CalculateOrbitMovement(Vector3 currentPosition,Vector3 centerPosition,float currentAngle,float orbitRadius,float orbitSpeed,float attractionForce,float resistance,float deltaTime)
    {
        Vector3 targetOrbitPos = centerPosition + new Vector3(Mathf.Cos(currentAngle) * orbitRadius, Mathf.Sin(currentAngle) * orbitRadius, 0f);
        Vector3 toTarget = targetOrbitPos - currentPosition;

        float currentDistance = Vector3.Distance(currentPosition, centerPosition);
        float radiusError = currentDistance - orbitRadius;
        Vector3 radialCorrection = (centerPosition - currentPosition).normalized * radiusError * attractionForce * 0.5f;

        Vector3 movement = (toTarget.normalized * orbitSpeed + radialCorrection) * deltaTime;
        movement *= (1f - resistance);

        return movement;
    }

    public static float CalculateOrbitAngleIncrement(float orbitSpeed, float deltaTime)
    {
        return orbitSpeed * deltaTime;
    }

    public static float CalculateAngleFromPosition(Vector3 relativePosition)
    {
        return Mathf.Atan2(relativePosition.y, relativePosition.x);
    }

    public static bool IsValidMagnetable(IMagnetable magnetable)
    {
        return magnetable != null && magnetable is Object obj && obj != null && magnetable.Transform != null;

        //if (magnetable == null)
        //    return false;
        //
        //if (magnetable.Transform == null)
        //    return false;
        //
        ////if (magnetable is MonoBehaviour mono && mono == null)
        ////    return false;
        //
        //return true;
    }
}