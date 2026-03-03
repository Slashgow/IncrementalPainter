using UnityEngine;

internal interface ILevelContainable
{
    public void ReverseDirection();
    public void ReflectDirection(Vector3 hitNormal);
}