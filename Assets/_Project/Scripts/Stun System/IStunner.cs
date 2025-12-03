using UnityEngine;
public interface IStunner
{
    float StunDuration { get; }
    float StunRadius { get; }
    void TryStun(Vector3 position);
}
