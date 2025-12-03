using System;

public interface IStunnable
{
    bool IsStunned { get; }
    float StunTimeRemaining { get; }
    void ApplyStun(float duration);

    event Action<float> OnStunApplied;
    event Action OnStunEnded;
}
