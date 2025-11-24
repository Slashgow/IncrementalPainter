using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SkillNode))]
public class SkillNodeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SkillNode skillNode = (SkillNode)target;

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Connection Tools", EditorStyles.boldLabel);

        if (skillNode.SkillData != null)
        {
            int requiredCount = skillNode.SkillData.RequiredSkills?.Count ?? 0;
            EditorGUILayout.HelpBox($"This skill requires {requiredCount} skill(s)", MessageType.Info);
        }

        if (GUILayout.Button("Create Connections to Required Skills", GUILayout.Height(30)))
        {
            skillNode.CreateConnectionsToRequiredSkills();
            EditorUtility.SetDirty(skillNode);
        }

        if (GUILayout.Button("Clear Connection Lines", GUILayout.Height(25)))
        {
            skillNode.ClearConnectionLines();
            EditorUtility.SetDirty(skillNode);
        }
    }
}

