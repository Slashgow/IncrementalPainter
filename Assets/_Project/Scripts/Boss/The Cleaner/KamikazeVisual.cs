using System;
using DG.Tweening;
using UnityEngine;

public class KamikazeVisual : MonoBehaviour
{
    [SerializeField] private KamikazeEnemy kamikazeEnemy;

    [Header("Tige")]
    [SerializeField] private SpriteRenderer tigeSpriteRenderer;

    [Header("Flash Body")]
    [SerializeField] private SpriteRenderer bodySspriteRenderer;
    [SerializeField] private Color warningColor = Color.red;
    [SerializeField, Range(0,30)] private int overshoot = 20;

    [Header("Boum")]
    [SerializeField] private Transform endLocalPositionBoum;
    [SerializeField] private Transform boumTransform;

    private Vector3 localPosition;
    private Material material;
    private Tween flashTween;
    private Tween boumMovement;

    private void Awake()
    {
        material = tigeSpriteRenderer.material;
        localPosition = boumTransform.localPosition;
    }

    private void OnEnable()
    {
        Reset();

        kamikazeEnemy.OnExplosionWarning += HandleExplosionWarning;

        flashTween = bodySspriteRenderer.DOColor(warningColor, kamikazeEnemy.TimeBeforeExploding).SetEase(Ease.Flash, overshoot, 0);
        boumMovement = boumTransform.DOLocalMove(endLocalPositionBoum.localPosition, kamikazeEnemy.TimeBeforeExploding).SetEase(Ease.Linear);
    }

    private void OnDisable()
    {
        kamikazeEnemy.OnExplosionWarning -= HandleExplosionWarning;
        flashTween?.Kill();
        boumMovement?.Kill();
    }

    private void HandleExplosionWarning(float percentage) => material.SetFloat("_Fill", percentage);

    private void Reset()
    {
        material.SetFloat("_Fill", 1f);
        bodySspriteRenderer.color = Color.white;
        boumTransform.localPosition = localPosition;
    }
}
