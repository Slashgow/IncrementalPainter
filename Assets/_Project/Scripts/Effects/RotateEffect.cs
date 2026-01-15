using DG.Tweening;
using inkolorgames.effects;
using UnityEngine;


public class RotateEffect : Effect
{
    [SerializeField, Range(0f, 10f)] private float rotationDuration = 1f;
    [SerializeField] private Ease ease = Ease.InOutSine;
    [SerializeField] private Vector3 rotationAxis = Vector3.up;
    [SerializeField, Range(-360f, 360f)] private float startAngle = 0f;
    [SerializeField, Range(-360f, 360f)] private float endAngle = 180f;
    [SerializeField] private RotateMode rotateMode = RotateMode.Fast;
    [SerializeField] private int loopCount = -1;
    [SerializeField] private LoopType loopType = LoopType.Restart;
    [SerializeField] private bool useRealTime = true;
    [SerializeField] private bool isRelative = false;
    [SerializeField] private bool forceReturnToOriginRotation = false;

    private Tween rotationTween;
    private Vector3 originRotation;

    protected override void OnEnable()
    {
        base.OnEnable();
        originRotation = transform.localEulerAngles;
    }
    public void Rotate()
    {
        rotationTween?.Kill();
        Vector3 currentRotation = transform.localEulerAngles;

        if (forceReturnToOriginRotation)
            currentRotation = originRotation;

        Vector3 normalizedAxis = rotationAxis.normalized;

        Vector3 startOffset = normalizedAxis * startAngle;
        Vector3 endOffset = normalizedAxis * endAngle;

        transform.localEulerAngles = currentRotation + startOffset;

        rotationTween = transform.DOLocalRotate(currentRotation + endOffset, rotationDuration, rotateMode)
            .SetEase(ease)
            .SetRelative(isRelative)
            .SetUpdate(useRealTime)
            .SetLoops(loopCount, loopType);
    }


    public override void DoEffect() => Rotate();
    private void OnDisable()
    {
        rotationTween?.Kill();
        transform.rotation = Quaternion.identity;
    }
}
