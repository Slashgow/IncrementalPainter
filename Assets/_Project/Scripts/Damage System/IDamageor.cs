using System;
using UnityEngine;

public interface IDamageor
{
    float Damage { get; }
    void TryDamage(Vector3 clickPosition);
}