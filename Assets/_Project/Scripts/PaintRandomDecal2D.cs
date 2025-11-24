using CW.Common;
using PaintIn2D;
using UnityEngine;

public class PaintRandomDecal2D : CwPaintDecal2D 
{
    [SerializeField] private Texture[] decalTextures;
    protected override Vector3 HandleHitCommon(bool preview, float pressure, int seed, Quaternion rotation)
    {
        var finalOpacity = opacity;
        var finalRadius = radius;
        var finalScale = scale;
        var finalColor = color;
        var finalAngle = angle;
        var finalTexture = decalTextures[Random.Range(0, decalTextures.Length - 1)];
        var finalMatrix = tileTransform != null ? tileTransform.localToWorldMatrix : Matrix4x4.identity;

        if (modifiers != null && modifiers.Count > 0)
        {
            CwHelper.BeginSeed(seed);
            modifiers.ModifyColor(ref finalColor, preview, pressure);
            modifiers.ModifyAngle(ref finalAngle, preview, pressure);
            modifiers.ModifyOpacity(ref finalOpacity, preview, pressure);
            modifiers.ModifyRadius(ref finalRadius, preview, pressure);
            modifiers.ModifyScale(ref finalScale, preview, pressure);
            modifiers.ModifyTexture(ref finalTexture, preview, pressure);
            CwHelper.EndSeed();
        }

        var finalAspect = PaintCore.CwCommon.GetAspect(shape, finalTexture);
        var finalSize = PaintCore.CwCommon.ScaleAspect(finalScale * finalRadius, finalAspect);

        CwCommandDecal2D.Instance.SetShape(rotation, finalSize, finalAngle);

        CwCommandDecal2D.Instance.SetMaterial(blendMode, finalTexture, shape, shapeChannel, finalColor, finalOpacity, tileTexture, finalMatrix, tileOpacity, tileTransition);

        return finalSize;
    }
}

#if UNITY_EDITOR
namespace PaintIn2D
{
    using PaintCore;
    using UnityEditor;
    using TARGET = PaintRandomDecal2D;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(TARGET))]
    public class CwPaintRandomDecal2D_Editor : CwEditor
    {
        protected override void OnInspector()
        {
            TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

            BeginError(Any(tgts, t => t.Layers == 0 && t.TargetModel == null));
            Draw("layers", "Only the CwModel/CwPaintableSprite GameObjects whose layers are within this mask will be eligible for painting.");
            EndError();
            Draw("group", "Only the CwPaintableTexture components with a matching group will be painted by this component.");

            Separator();

            Draw("blendMode", "This allows you to choose how the paint from this component will combine with the existing pixels of the textures you paint.\n\nNOTE: See the Blend Mode section of the documentation for more information.");
            BeginError(Any(tgts, t => t.Texture == null && t.Shape == null));
            Draw("texture", "The texture that will be painted.");
            EndError();
            EditorGUILayout.BeginHorizontal();
            BeginError(Any(tgts, t => t.BlendMode.Index == CwBlendMode.REPLACE && t.Shape == null));
            Draw("shape", "This allows you to specify the shape of the texture. This is optional for most blending modes, because they usually derive their shape from the RGB or A values. However, if you're using the Replace blending mode, then you must manually specify the shape.");
            EndError();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("shapeChannel"), GUIContent.none, GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();
            Draw("color", "The color of the paint.");
            Draw("opacity", "The opacity of the brush.");

            Separator();

            Draw("angle", "The angle of the texture in degrees.");
            Draw("scale", "This allows you to control the mirroring and aspect ratio of the texture.\n\n1, 1 = No scaling.\n-1, 1 = Horizontal Flip.");
            BeginError(Any(tgts, t => t.Radius <= 0.0f));
            Draw("radius", "The radius of the paint brush.");
            EndError();

            Separator();

            if (DrawFoldout("Advanced", "Show advanced settings?") == true)
            {
                BeginIndent();
                Draw("targetModel", "If this is set, then only the specified CwModel/CwPaintableSprite will be painted, regardless of the layer setting.");
                Draw("targetTexture", "If this is set, then only the specified CwPaintableTexture will be painted, regardless of the layer or group setting.");
                Draw("findMask", "If your scene contains a <b>CwMask</b>, should this paint component use it?");

                Separator();

                Draw("tileTexture", "This allows you to apply a tiled detail texture to your texture. This tiling will be applied in world space using triplanar mapping.");
                Draw("tileTransform", "This allows you to adjust the tiling position + rotation + scale using a Transform.");
                Draw("tileOpacity", "This allows you to control the triplanar influence.\n\n0 = No influence.\n\n1 = Full influence.");
                Draw("tileTransition", "This allows you to control how quickly the triplanar mapping transitions between the X/Y/Z planes.");
                EndIndent();
            }

            Separator();
            Draw("decalTextures", "The different decal textures");

            tgt.Modifiers.DrawEditorLayout(serializedObject, target, "Color", "Angle", "Opacity", "Radius", "Scale", "Texture", "Position");
        }
    }
}
#endif