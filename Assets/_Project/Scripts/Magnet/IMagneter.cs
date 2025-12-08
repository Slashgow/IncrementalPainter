using UnityEngine;

public interface IMagneter
{
    Transform Transform { get; }
    float AttractionForce { get; }
    float AttractionRadius { get; }
    float OrbitRadius { get; }
    float OrbitSpeed { get; }
    bool IsActive { get; }

    void AddMagnetableObject(IMagnetable magnetable);
    void RemoveMagnetableObject(IMagnetable magnetable);
}