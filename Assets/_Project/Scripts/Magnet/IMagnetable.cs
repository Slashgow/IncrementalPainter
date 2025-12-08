using System;
using UnityEngine;

public interface IMagnetable
{
    Transform Transform { get; }
    bool IsBeingMagneted { get; }
    float MagnetResistance { get; } // 0 = no resistance, 1 = full resistance

    void ApplyMagneticMovement(Vector3 movement);
    void OnMagnetEnter(IMagneter magneter);
    void OnMagnetExit(IMagneter magneter);

    event Action<IMagneter> OnMagnetEntered;
    event Action<IMagneter> OnMagnetExited;
}
