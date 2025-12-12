using System.Collections.Generic;
using UnityEngine;

public class BombExplosionParticle : MonoBehaviour
{
    [SerializeField] private List<ParticleSystem> smokes = new();
    [SerializeField] private ParticleSystem impactParticleSystem, circleParticleSystem;

    private ParticleSystemRenderer impactRenderer;
    private ParticleSystemRenderer circleRenderer;

    private void Awake()
    {
        impactRenderer = impactParticleSystem.GetComponent<ParticleSystemRenderer>();
        circleRenderer = circleParticleSystem.GetComponent<ParticleSystemRenderer>();
    }

    public void Initialize(Color color)
    {
        foreach (var smoke in smokes)
        {
            var main = smoke.main;
            main.startColor = color;
        }
        impactRenderer.material.SetColor("_BaseColor", color);
        circleRenderer.material.SetColor("_BaseColor", color);
    }
}
