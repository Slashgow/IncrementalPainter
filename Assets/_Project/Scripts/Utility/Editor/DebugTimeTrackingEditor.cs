#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DebugTimeTracking))]
public class DebugTimeTrackingEditor : Editor
{
    private DebugTimeTracking _target;
    private bool _levelsFoldout = true;

    private void OnEnable() => _target = (DebugTimeTracking)target;

    public override void OnInspectorGUI()
    {
        if (Application.isPlaying) Repaint();

        // ?? Main Menu ??????????????????????????????????????????????????
        EditorGUILayout.Space(4);
        EditorGUILayout.LabelField("?? Main Menu", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Main Menu", EditorStyles.boldLabel, GUILayout.Width(160));
        EditorGUILayout.LabelField(FormatTime(_target.GetMainMenuTime()));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();

        // ?? GameStates ?????????????????????????????????????????????????
        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("? Game States", EditorStyles.boldLabel);

        float total = _target.GetTotalStateTime();
        foreach (GameManager.GameState state in Enum.GetValues(typeof(GameManager.GameState)))
        {
            float t = _target.GetTimeInState(state);
            float pct = total > 0f ? t / total : 0f;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(state.ToString(), EditorStyles.boldLabel, GUILayout.Width(160));
            EditorGUILayout.LabelField(FormatTime(t), GUILayout.Width(70));
            EditorGUILayout.LabelField($"{pct * 100f:F1}%", GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();

            Rect rect = GUILayoutUtility.GetRect(0, 6, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(rect, new Color(0.2f, 0.2f, 0.2f));
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width * pct, rect.height), StateColor(state));
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(2);
        }

        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        EditorGUILayout.LabelField("TOTAL", EditorStyles.boldLabel, GUILayout.Width(160));
        EditorGUILayout.LabelField(FormatTime(total));
        EditorGUILayout.EndHorizontal();

        // ?? Levels ?????????????????????????????????????????????????????
        EditorGUILayout.Space(8);
        _levelsFoldout = EditorGUILayout.Foldout(_levelsFoldout, "?? Levels (this session)", true, EditorStyles.foldoutHeader);
        if (_levelsFoldout)
        {
            var levelsProp = serializedObject.FindProperty("_levelTimings");
            if (levelsProp != null && levelsProp.arraySize > 0)
            {
                // Find max for relative bar sizing
                float maxLevel = 0f;
                for (int i = 0; i < levelsProp.arraySize; i++)
                {
                    float s = levelsProp.GetArrayElementAtIndex(i).FindPropertyRelative("seconds").floatValue;
                    if (s > maxLevel) maxLevel = s;
                }

                for (int i = 0; i < levelsProp.arraySize; i++)
                {
                    var elem = levelsProp.GetArrayElementAtIndex(i);
                    string title = elem.FindPropertyRelative("levelTitle").stringValue;
                    float secs = elem.FindPropertyRelative("seconds").floatValue;
                    float pct = maxLevel > 0f ? secs / maxLevel : 0f;

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(title, EditorStyles.boldLabel, GUILayout.Width(160));
                    EditorGUILayout.LabelField(FormatTime(secs));
                    EditorGUILayout.EndHorizontal();

                    Rect rect = GUILayoutUtility.GetRect(0, 6, GUILayout.ExpandWidth(true));
                    EditorGUI.DrawRect(rect, new Color(0.2f, 0.2f, 0.2f));
                    EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width * pct, rect.height), new Color(0.26f, 0.65f, 0.80f));
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space(2);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("No level played yet this session.", MessageType.None);
            }
        }

        // ?? Buttons ????????????????????????????????????????????????????
        EditorGUILayout.Space(8);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("?? Log to Console")) _target.LogTimeTracking();
        if (GUILayout.Button("?? Reset")) _target.ResetTimeTracking();
        EditorGUILayout.EndHorizontal();

        serializedObject.Update();
    }

    private static string FormatTime(float seconds)
    {
        int m = (int)(seconds / 60f);
        int s = (int)(seconds % 60f);
        return $"{m}m {s:D2}s";
    }

    private static Color StateColor(GameManager.GameState state) => state switch
    {
        GameManager.GameState.PAINT => new Color(0.27f, 0.65f, 0.36f),
        GameManager.GameState.DAY_SUMMARY => new Color(0.26f, 0.52f, 0.80f),
        GameManager.GameState.UPGRADE => new Color(0.80f, 0.60f, 0.20f),
        GameManager.GameState.GALLERY => new Color(0.60f, 0.30f, 0.80f),
        GameManager.GameState.BOSS_INTRODUCTION => new Color(0.85f, 0.25f, 0.25f),
        _ => Color.gray
    };
}
#endif