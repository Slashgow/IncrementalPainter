using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillNode : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SkillData skillData;
    public SkillData SkillData => skillData;
    [SerializeField] private Image iconImage;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Button button;

    [Header("Connection Lines")]
    [SerializeField] private List<LineRenderer> connectionLines;
    public List<LineRenderer> ConnectionLines => connectionLines;

    private SkillTreeManager treeManager;
    private SkillState currentState;

    public enum SkillState
    {
        Locked,
        Available,
        Unlocked
    }

    void Start()
    {
        treeManager = GetComponentInParent<SkillTreeManager>();
        button.onClick.AddListener(OnSkillClicked);
        Initialize();
    }

    public void Initialize()
    {
        if (iconImage) 
            iconImage.sprite = skillData.Icon;
        if (nameText) 
            nameText.text = skillData.SkillName;
        if (costText) 
            costText.text = $"Cost: {skillData.SkillPointCost} SP";

        UpdateVisuals();
    }

    void OnSkillClicked()
    {
        if (treeManager != null)
        {
            treeManager.TryUnlockSkill(skillData);
        }
    }

    public void UpdateVisuals()
    {
        if (skillData == null) 
            return;

        currentState = DetermineState();

        switch (currentState)
        {
            case SkillState.Locked:
                if (backgroundImage) 
                    backgroundImage.color = skillData.LockedColor;
                if (button) 
                    button.interactable = false;
                break;

            case SkillState.Available:
                if (backgroundImage) 
                    backgroundImage.color = skillData.AvailableColor;
                if (button) 
                    button.interactable = true;
                break;

            case SkillState.Unlocked:
                if (backgroundImage) 
                    backgroundImage.color = skillData.UnlockedColor;
                if (button) 
                    button.interactable = false;
                break;
        }

        UpdateConnectionLines();
    }

    SkillState DetermineState()
    {
        if (treeManager.IsSkillUnlocked(skillData.SkillID))
            return SkillState.Unlocked;

        if (treeManager.CanUnlockSkill(skillData))
            return SkillState.Available;

        return SkillState.Locked;
    }

    void UpdateConnectionLines()
    {
        foreach (var line in connectionLines)
        {
            if (line != null)
            {
                Color lineColor = currentState == SkillState.Unlocked ? skillData.UnlockedColor : skillData.LockedColor;
                lineColor.a = 0.5f;
                line.startColor = lineColor;
                line.endColor = lineColor;
            }
        }
    }

#if UNITY_EDITOR
    public void CreateConnectionLine(SkillNode targetNode)
    {
        GameObject lineObj = new GameObject($"Line_{skillData.SkillName}_to_{targetNode.skillData.SkillName}");
        lineObj.transform.SetParent(transform.parent);

        LineRenderer line = lineObj.AddComponent<LineRenderer>();
        line.positionCount = 2;
        line.startWidth = 3f;
        line.endWidth = 3f;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.sortingOrder = -1;

        line.SetPosition(0, transform.position);
        line.SetPosition(1, targetNode.transform.position);

        connectionLines.Add(line);
    }

    [ContextMenu("Create Connections to Required Skills")]
    public void CreateConnectionsToRequiredSkills()
    {
        if (skillData == null || skillData.RequiredSkills == null || skillData.RequiredSkills.Count == 0)
        {
            Debug.Log($"{skillData?.SkillName ?? "Skill"} has no required skills");
            return;
        }

        ClearConnectionLines();

        if (treeManager == null)
            treeManager = GetComponentInParent<SkillTreeManager>();

        if (treeManager == null)
        {
            Debug.LogError("SkillTreeManager not found!");
            return;
        }

        foreach (var requiredSkill in skillData.RequiredSkills)
        {
            if (requiredSkill != null)
            {
                SkillNode requiredNode = treeManager.FindNodeBySkillData(requiredSkill);
                if (requiredNode != null)
                    CreateConnectionLine(requiredNode);
                else
                    Debug.LogWarning($"Could not find node for required skill: {requiredSkill.SkillName}");
            }
        }
    }

    [ContextMenu("Clear Connection Lines")]
    public void ClearConnectionLines()
    {
        foreach (var line in connectionLines)
        {
            if (line != null)
                DestroyImmediate(line.gameObject);
        }
        connectionLines.Clear();
    }
#endif
}
