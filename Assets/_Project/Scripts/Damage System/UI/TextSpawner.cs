using inkolorgames;
using UnityEngine;

public abstract class TextSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PoolingSystem poolingSystem;
    [SerializeField] private Camera targetCamera;
    [SerializeField] private Transform damageTextParent;

    [Header("Offset Settings")]
    [SerializeField] private Vector2 offset = Vector2.zero;
    [SerializeField] private bool useRandomOffset = false;
    [SerializeField] private Vector2 randomOffsetRange = new Vector2(20f, 20f);

    [Header("Text Settings")]
    [SerializeField] private Color normalHitColor = Color.white;
    [SerializeField] private Color criticalHitColor = Color.yellow;

    protected virtual void SpawnDamageText(float damageAmount, Vector3 worldPosition, bool isCritical) =>
        SpawnDamageText(damageAmount, worldPosition, isCritical, string.Empty);
    protected virtual void SpawnDamageText(float damageAmount, Vector3 worldPosition, bool isCritical, string textAfterAmount = "")
    {
        Vector3 screenPosition = targetCamera.WorldToScreenPoint(worldPosition);
        Vector2 finalOffset = Helper.CalculateOffset(useRandomOffset, offset, randomOffsetRange);

        screenPosition += new Vector3(finalOffset.x, finalOffset.y, 0f);

        GameObject damageTextObject = poolingSystem.GetPrefabFromPool(screenPosition, damageTextParent, true);

        var damageText = damageTextObject.GetComponent<DamageText>();

        if (isCritical)
            damageText.InitializeCritical(damageAmount, criticalHitColor, poolingSystem, textAfterAmount);
        else
            damageText.Initialize(damageAmount, normalHitColor, poolingSystem, textAfterAmount);
    }
}
