using UnityEngine;
public interface ISplitter
{
    int SplitCount { get; }
    bool IsActive { get; }

    void TrySplit(ISplittable splittable, Vector3 splitOrigin);
}