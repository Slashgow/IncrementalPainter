using System;
using UnityEngine;
using UnityEngine.Events;
using UnityTimer;

public class SimpleSplittable : MonoBehaviour, ISplittable
{
    [Header("Split Settings")]
    [SerializeField, Range(0,3)] private int maxSplitGenerations = 1;
    [SerializeField, Range(0f, 2f)] private float minSpawnRadius = 0f;
    [SerializeField, Range(0f, 2f)] private float maxSpawnRadius = 1.0f;
    [SerializeField] private bool canSplit = true;
    private int currentSplitGeneration = 0;

    [Header("Split Visuals")]
    [SerializeField, Range(0f,1f)] private float scaleMultiplier = 0.7f;

    private Timer movementTimer;

    public Transform Transform => transform;

    public float ScaleMultiplier => scaleMultiplier;
    public bool CanSplit => canSplit && currentSplitGeneration < maxSplitGenerations;
    public int CurrentSplitGeneration => currentSplitGeneration;
    public int MaxSplitGenerations => maxSplitGenerations;

    public UnityEvent<int> OnSplitUnityEvent;
    public event Action<ISplittable, int> OnSplit;

    public static event Action<ISplittable, GameObject[], Vector3> OnAnySplittableSplit;

    public void Split(int splitCount, Vector3 splitOrigin)
    {
        if (!CanSplit || splitCount <= 0)
            return;

        Vector3[] positions = SplitUtility.CalculateRandomSpawnPositions(transform.position, splitCount, minSpawnRadius, maxSpawnRadius);
        float newScale = SplitUtility.CalculateSplitScale(transform.localScale.x, currentSplitGeneration + 1, scaleMultiplier);

        GameObject[] spawnedObjects = new GameObject[splitCount];

        for (int i = 0; i < splitCount; i++)
        {
            Vector3 dir = (positions[i] - transform.position).normalized;

            var blob = PaintSpawner.Instance.SpawnPaintBlob(positions[i], PaintType.Split, currentSplitGeneration + 1, newScale);
            spawnedObjects[i] = blob;
        }

        OnSplit?.Invoke(this, splitCount);
        OnSplitUnityEvent?.Invoke(splitCount);
        OnAnySplittableSplit?.Invoke(this, spawnedObjects, transform.position);

        this.GetComponent<IDamageable>().Die();
    }


    public void SetGeneration(int generation) => currentSplitGeneration = generation;

    private void OnDestroy()
    {
        if (movementTimer != null && !movementTimer.isDone)
            movementTimer.Cancel();
    }
}