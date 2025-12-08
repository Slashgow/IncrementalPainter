using System;
using UnityEngine;

public interface ISplittable
{
    Transform Transform { get; }
    bool CanSplit { get; }
    int CurrentSplitGeneration { get; }
    int MaxSplitGenerations { get; }

    void Split(int splitCount, Vector3 splitOrigin);

    event Action<ISplittable, int> OnSplit;
}
