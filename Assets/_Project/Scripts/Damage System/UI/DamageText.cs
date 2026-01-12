using DG.Tweening;
using inkolorgames;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI textComponent;

    [Header("Animation Settings")]
    [SerializeField] private float animationDuration = 1f;
    [SerializeField] private float moveUpDistance = 50f;
    [SerializeField] private AnimationCurve movementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Critical Hit Settings")]
    [SerializeField] private float criticalScaleMultiplier = 1.5f;
    [SerializeField] private float criticalPunchScale = 0.2f;

    private PoolingSystem poolingSystem;
    private Sequence animationSequence;
    private float originalFontSize;

    public void Initialize(float damageAmount, Color textColor, PoolingSystem pool, string textAfterAmount = "")
    {
        poolingSystem = pool;

        if(originalFontSize == 0f)
            originalFontSize = textComponent.fontSize;

        textComponent.fontSize = originalFontSize * AdvancedGameSettingsManager.Instance.FontSizeMultiplier;
        textComponent.text = $"{Mathf.RoundToInt(damageAmount)} {textAfterAmount}";
        textComponent.color = textColor;
        transform.localScale = Vector3.one;

        PlayAnimation(false);
    }

    public void InitializeCritical(float damageAmount, Color textColor, PoolingSystem pool, string textAfterAmount = "")
    {
        poolingSystem = pool;

        if (originalFontSize == 0f)
            originalFontSize = textComponent.fontSize;

        textComponent.fontSize = originalFontSize * AdvancedGameSettingsManager.Instance.FontSizeMultiplier;
        textComponent.text = $"{Mathf.RoundToInt(damageAmount)} {textAfterAmount}";
        textComponent.color = textColor;

        transform.localScale = Vector3.one * criticalScaleMultiplier;

        PlayAnimation(true);
    }

    private void PlayAnimation(bool isCritical)
    {
        animationSequence?.Kill();

        Vector3 startPos = transform.localPosition;
        Vector3 endPos = startPos + new Vector3(0f, moveUpDistance, 0f);

        animationSequence = DOTween.Sequence();

        animationSequence.Append(transform.DOLocalMove(endPos, animationDuration).SetEase(movementCurve));

        if (isCritical)
            animationSequence.Join(transform.DOPunchScale(Vector3.one * criticalPunchScale, animationDuration * 0.75f, 2, 0.5f));

        animationSequence.OnComplete(() => ReturnToPool());
    }

    private void ReturnToPool()
    {
        if (poolingSystem != null)
            poolingSystem.AddToPool(gameObject);
        else
            gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        animationSequence?.Kill();
    }
}
