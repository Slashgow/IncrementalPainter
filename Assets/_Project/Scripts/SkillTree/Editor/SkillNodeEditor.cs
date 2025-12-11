using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SkillNode))]
public class SkillNodeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SkillNode skillNode = (SkillNode)target;
        SkillDataBase skillData = skillNode.SkillDataBase;

        EditorGUILayout.Space(15);
        EditorGUILayout.LabelField("Skill Overview", EditorStyles.boldLabel);

        if (skillData == null)
        {
            EditorGUILayout.HelpBox("No SkillData assigned!", MessageType.Error);
            return;
        }

        // Current runtime state (live preview)
        int currentLevel = 0;
        int maxLevel = skillData.MaxLevel;
        bool isInitialized = skillNode.TreeManager != null;

        if (isInitialized)
        {
            currentLevel = skillNode.TreeManager.GetSkillLevel(skillData.SkillID);
        }

        // Header box with skill info
        EditorGUILayout.BeginVertical("box");
        {
            EditorGUILayout.LabelField(skillData.SkillName, EditorStyles.largeLabel);

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Current Level:", GUILayout.Width(100));
            if (isInitialized)
                EditorGUILayout.LabelField($"{currentLevel} / {maxLevel}", EditorStyles.boldLabel);
            else
            {
                EditorGUILayout.LabelField($"{skillData.StartingLevel} (not in game yet)", EditorStyles.miniLabel);
                GUILayout.EndHorizontal();

                float currentValue = skillData.GetEffectValueAtLevel(currentLevel);
                float nextValue = skillData.GetEffectValueAtLevel(currentLevel + 1);

                EditorGUILayout.LabelField("Current Value:", $"{currentValue:F2}");
                if (currentLevel < maxLevel)
                {
                    EditorGUILayout.LabelField("Next Level Value:", $"<b>{nextValue:F2}</b>", new GUIStyle(EditorStyles.label) { richText = true });
                }
                else
                {
                    EditorGUILayout.LabelField("Status:", "<color=#FFD700><b>MAXED</b></color>", new GUIStyle(EditorStyles.label) { richText = true });
                }
            }
                  
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(10);

        // Show all level requirements summary
        EditorGUILayout.LabelField("All Level Requirements", EditorStyles.boldLabel);

        if (skillData.LevelRequirements == null || skillData.LevelRequirements.Count == 0)
        {
            EditorGUILayout.HelpBox("No level requirements defined.", MessageType.Info);
        }
        else
        {
            foreach (var req in skillData.LevelRequirements)
            {
                if (req == null) continue;

                bool isUnlocked = currentLevel >= req.Level;
                bool canAffordNext = isInitialized && skillNode.TreeManager.CanLevelUpToLevel(skillData, req.Level);

                string status = isUnlocked ? "UNLOCKED" : (canAffordNext ? "AVAILABLE" : "LOCKED");

                Color statusColor = isUnlocked ? Color.green : (canAffordNext ? new Color(1f, 0.8f, 0f) : Color.red);

                EditorGUILayout.BeginVertical("box");
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"Level {req.Level}", EditorStyles.boldLabel, GUILayout.Width(80));

                    GUI.color = statusColor;
                    EditorGUILayout.LabelField(status, GUILayout.Width(80));
                    GUI.color = Color.white;

                    if (req.SkillPointCost > 0 || skillData.GetCostForLevel(req.Level) > 0)
                    {
                        string costStr = "";
                        if (skillData.GetCostForLevel(req.Level) > 0)
                            costStr += $"{skillData.GetCostForLevel(req.Level):N0} $";
                        if (req.SkillPointCost > 0)
                            costStr += (costStr.Length > 0 ? " + " : "") + $"{req.SkillPointCost} SP";

                        EditorGUILayout.LabelField(costStr, EditorStyles.miniLabel);
                    }
                    GUILayout.EndHorizontal();

                    // Required skills
                    if (req.RequiredSkills != null && req.RequiredSkills.Count > 0)
                    {
                        EditorGUI.indentLevel++;
                        foreach (var r in req.RequiredSkills)
                        {
                            if (r.SkillData != null)
                            {
                                string reqText = $"• {r.SkillData.SkillName} Lv.{r.RequiredLevel}";
                                if (isInitialized)
                                {
                                    int actualLevel = skillNode.TreeManager.GetSkillLevel(r.SkillData.SkillID);
                                    bool met = actualLevel >= r.RequiredLevel;
                                    EditorGUILayout.LabelField(reqText + (met ? " (OK)" : $" ({actualLevel}/{r.RequiredLevel})"),
                                        met ? EditorStyles.label : new GUIStyle(EditorStyles.label) { normal = { textColor = Color.red } });
                                }
                                else
                                {
                                    EditorGUILayout.LabelField(reqText);
                                }
                            }
                        }
                        EditorGUI.indentLevel--;
                    }
                }
                EditorGUILayout.EndVertical();
            }
        }

        EditorGUILayout.Space(15);
        EditorGUILayout.LabelField("Connection Tools", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Create Connections to Required Skills", GUILayout.Height(35)))
            {
                skillNode.CreateConnectionsToRequiredSkills();
                EditorUtility.SetDirty(skillNode);
                SceneView.RepaintAll();
            }

            if (GUILayout.Button("Clear Lines", GUILayout.Height(35)))
            {
                skillNode.ClearConnectionLines();
                EditorUtility.SetDirty(skillNode);
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(10);

        // Debug buttons
        if (Application.isPlaying && isInitialized)
        {
            EditorGUILayout.LabelField("Runtime Debug", EditorStyles.boldLabel);

            if (GUILayout.Button("Force Level Up (Debug)", GUILayout.Height(30)))
            {
                skillNode.TreeManager.TryLevelUpSkill(skillData);

                if (GUILayout.Button("Reset This Skill to Level 0", GUILayout.Height(30)))
                {
                    var skillLevelData = skillNode.TreeManager.GetType()
                        .GetField("leveledSkills", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                        ?.GetValue(skillNode.TreeManager) as Dictionary<string, ISkillLevelData>;

                    if (skillLevelData != null && skillLevelData.TryGetValue(skillData.SkillID, out var data))
                    {
                        while (data.CurrentLevel > skillData.StartingLevel)
                            data.LevelUp(); // Go backwards? No — reset properly
                                            // Actually reset:
                        typeof(SkillDataPerLevelOfType<>).MakeGenericType(data.GetType().GetGenericArguments()[0])
                            .GetMethod("Initialize")?.Invoke(data, null);
                        skillNode.TreeManager.RefreshAllNodes();
                    }
                }
            }

            // Force refresh button
            if (GUILayout.Button("Refresh Visuals (Editor)", GUILayout.Height(25)))
            {
                if (Application.isPlaying)
                    skillNode.UpdateVisuals();
            }
        }
    }
}
