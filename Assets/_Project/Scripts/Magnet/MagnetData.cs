using UnityEngine;

[System.Serializable]
public class MagnetData
{
    [Header("Magnet Properties")]
    [SerializeField, Range(0f,3f)] private float attractionForce = 2f;
    [SerializeField, Range(0f,10f)] private float attractionRadius = 5f;
    [SerializeField, Range(0f,2.5f)] private float orbitRadius = 1f;
    [SerializeField, Range(0f,4f)] private float orbitSpeed = 2f;

    public float AttractionForce => attractionForce;
    public float AttractionRadius => attractionRadius;
    public float OrbitRadius => orbitRadius;
    public float OrbitSpeed => orbitSpeed;

    public MagnetData(float force, float radius, float orbit, float speed)
    {
        attractionForce = force;
        attractionRadius = radius;
        orbitRadius = orbit;
        orbitSpeed = speed;
    }

    public static MagnetData FromMagneterManager()
    {
        if (!MagneterManager.HasInstance)
        {
            Debug.LogWarning("MagneterManager instance not found. Using default values.");
            return new MagnetData(10f, 5f, 1f, 2f);
        }

        return new MagnetData(
            MagneterManager.Instance.AttractionForcePerLevel.GetCurrentLevelData(),
            MagneterManager.Instance.AttractionRadiusPerLevel.GetCurrentLevelData(),
            MagneterManager.Instance.OrbitRadiusPerLevel.GetCurrentLevelData(),
            MagneterManager.Instance.OrbitSpeedPerLevel.GetCurrentLevelData()
        );
    }
}