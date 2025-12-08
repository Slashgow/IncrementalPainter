using System;
using UnityEngine;
using UnityEngine.Events;

public class Splitter : MonoBehaviour, ISplitter
{
    [Header("Splitter Configuration")]
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> splitCountPerLevel;

    public SkillDataPerLevelOfType<FunctionAffine> SplitCountPerLevel => splitCountPerLevel;

    [Header("Settings")]
    [SerializeField] private bool isActive = true;

    public int SplitCount => Mathf.RoundToInt(splitCountPerLevel.GetCurrentLevelData());
    public bool IsActive => isActive;

    public UnityEvent OnSplitterActivated;
    public UnityEvent OnSplitterDeactivated;
    public UnityEvent<ISplittable> OnSplitTriggered;

    public static event Action<ISplitter, ISplittable, int> OnAnySplitterTrigger;

    private void Start() => PaintBlob.OnAnyPaintBlobSplitDie += PaintBlob_OnAnyPaintBlobSplitDie;
    private void OnDestroy() => PaintBlob.OnAnyPaintBlobSplitDie -= PaintBlob_OnAnyPaintBlobSplitDie;

    private void PaintBlob_OnAnyPaintBlobSplitDie(Vector3 deathWorldPosition, ISplittable splittable)
    {
        if (!isActive)
            return;

        TrySplit(splittable, deathWorldPosition);
    }

    public void TrySplit(ISplittable splittable, Vector3 splitOrigin)
    {
        if (!isActive)
            return;

        if (!SplitUtility.IsValidSplittable(splittable))
            return;

        if (!splittable.CanSplit)
            return;

        int actualSplitCount = SplitCount;
        if (actualSplitCount <= 0)
            return;

        splittable.Split(actualSplitCount, splitOrigin);

        OnSplitTriggered?.Invoke(splittable);
        OnAnySplitterTrigger?.Invoke(this, splittable, actualSplitCount);
    }

    public void SetActive(bool active)
    {
        bool wasActive = isActive;
        isActive = active;

        if (isActive && !wasActive)
        {
            OnSplitterActivated?.Invoke();
        }
        else if (!isActive && wasActive)
        {
            OnSplitterDeactivated?.Invoke();
        }
    }
}