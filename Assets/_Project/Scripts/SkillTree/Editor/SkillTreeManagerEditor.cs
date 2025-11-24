using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SkillTreeManager))]
public class SkillTreeManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SkillTreeManager manager = (SkillTreeManager)target;

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Skill Tree Connection Tools", EditorStyles.boldLabel);

        int totalNodes = manager.allSkillNodes?.Count ?? 0;
        int totalLines = 0;
        if (manager.allSkillNodes != null)
        {
            foreach (var node in manager.allSkillNodes)
            {
                if (node != null)
                    totalLines += node.ConnectionLines.Count;
            }
        }

        EditorGUILayout.HelpBox($"Total Skill Nodes: {totalNodes}\nTotal Connection Lines: {totalLines}", MessageType.Info);

        EditorGUILayout.Space(5);

        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Create All Connection Lines", GUILayout.Height(40)))
        {
            manager.CreateAllConnectionLines();
            EditorUtility.SetDirty(manager);

            foreach (var node in manager.allSkillNodes)
            {
                if (node != null)
                    EditorUtility.SetDirty(node);
            }
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.Space(3);

        GUI.backgroundColor = new Color(1f, 0.7f, 0.7f);
        if (GUILayout.Button("Clear All Connection Lines", GUILayout.Height(30)))
        {
            if (EditorUtility.DisplayDialog("Clear All Lines?",
                "This will remove all connection lines from all skill nodes. Continue?",
                "Yes", "Cancel"))
            {
                manager.ClearAllConnectionLines();
                EditorUtility.SetDirty(manager);

                foreach (var node in manager.allSkillNodes)
                {
                    if (node != null)
                        EditorUtility.SetDirty(node);
                }
            }
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Testing Tools", EditorStyles.boldLabel);

        if (GUILayout.Button("Reset Skill Tree (Runtime Only)", GUILayout.Height(25)))
        {
            if (Application.isPlaying)
            {
                manager.ResetSkillTree();
                Debug.Log("Skill tree reset");
            }
            else
            {
                EditorUtility.DisplayDialog("Runtime Only",
                    "This button only works in Play Mode", "OK");
            }
        }
    }
}