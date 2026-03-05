using UnityEngine;

public class MeshRendererSRPBatchIncompatible : MonoBehaviour
{
    [SerializeField] private new Renderer renderer;

    private static MaterialPropertyBlock sharedMPB;

    private void OnEnable()
    {
        // Lazy initialization: allowed by Unity
        if (sharedMPB == null)
            sharedMPB = new MaterialPropertyBlock();

        // Assigning an MPB makes this renderer incompatible with SRP Batcher
        // and clears any previous per-instance overrides
        renderer.SetPropertyBlock(sharedMPB);
    }

}
