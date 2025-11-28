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
        EditorGUILayout.LabelField("Level Information", EditorStyles.boldLabel);

        if (skillNode.SkillDataBase != null)
        {
            // Display level-specific information
            var levelReq = skillNode.SkillDataBase.GetRequirementsForLevel(skillNode.TargetLevel);

            if (levelReq != null)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField($"Level {skillNode.TargetLevel} Requirements:", EditorStyles.boldLabel);

                if (skillNode.SkillDataBase.GetCostForLevel(skillNode.TargetLevel) > 0)
                    EditorGUILayout.LabelField($"Currency: {skillNode.SkillDataBase.GetCostForLevel(skillNode.TargetLevel)}");

                // Show required skills
                int requiredCount = levelReq.RequiredSkills?.Count ?? 0;
                EditorGUILayout.LabelField($"Required Skills: {requiredCount}");

                if (requiredCount > 0)
                {
                    EditorGUI.indentLevel++;
                    foreach (var reqSkill in levelReq.RequiredSkills)
                    {
                        if (reqSkill != null)
                            EditorGUILayout.LabelField($"• {reqSkill.SkillData.SkillName}");
                    }
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.EndVertical();
            }
            else
            {
                EditorGUILayout.HelpBox($"No specific requirements defined for Level {skillNode.TargetLevel}", MessageType.Warning);
            }

            // Show skill range
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField($"Skill Level Range: {skillNode.SkillDataBase.StartingLevel} - {skillNode.SkillDataBase.MaxLevel}");

            // Show effect value at this level
            float effectValue = skillNode.SkillDataBase.GetEffectValueAtLevel(skillNode.TargetLevel);
            EditorGUILayout.LabelField($"Effect Value at Level {skillNode.TargetLevel}: {effectValue:F2}");
        }

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Connection Tools", EditorStyles.boldLabel);

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

        EditorGUILayout.Space(5);

        // Quick level navigation buttons
        if (skillNode.SkillDataBase != null)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Quick Level Change:", GUILayout.Width(120));

            if (GUILayout.Button("-", GUILayout.Width(30)))
            {
                if (skillNode.TargetLevel > skillNode.SkillDataBase.StartingLevel)
                {
                    SerializedProperty targetLevelProp = serializedObject.FindProperty("targetLevel");
                    targetLevelProp.intValue--;
                    serializedObject.ApplyModifiedProperties();
                }
            }

            EditorGUILayout.LabelField($"Level {skillNode.TargetLevel}", EditorStyles.centeredGreyMiniLabel);

            if (GUILayout.Button("+", GUILayout.Width(30)))
            {
                if (skillNode.TargetLevel < skillNode.SkillDataBase.MaxLevel)
                {
                    SerializedProperty targetLevelProp = serializedObject.FindProperty("targetLevel");
                    targetLevelProp.intValue++;
                    serializedObject.ApplyModifiedProperties();
                }
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}