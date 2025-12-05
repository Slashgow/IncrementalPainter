using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillNodeDetail : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image backgroundTitleImage;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI statValueDescriptionText;

    private SkillNode skillNode;

    private string valueHexaColorLocked;
    private string valueHexaColorAvailable;
    private string valueHexaUnlocked;

    private string currentHexaColor;
    private Color currentColorBackground;
    private Color currentColorBackgroundDarker;


    public void Initialize(SkillNode node)
    {
        skillNode = node;

        valueHexaColorAvailable = ColorUtility.ToHtmlStringRGB(skillNode.TreeManager.SkillTreeVisualData.AvailableColorDarker);
        valueHexaUnlocked = ColorUtility.ToHtmlStringRGB(skillNode.TreeManager.SkillTreeVisualData.UnlockedColorDarker);
        valueHexaColorLocked = ColorUtility.ToHtmlStringRGB(skillNode.TreeManager.SkillTreeVisualData.LockedColorDarker);

        UpdateVisual();
    }

    public void UpdateVisual()
    {
        UpdateCurrentColor();

        if (descriptionText)
            descriptionText.text = skillNode.SkillDataBase.Description.GetLocalizedString();
        if (statValueDescriptionText)
            statValueDescriptionText.text = $"<color=#{currentHexaColor}>{skillNode.SkillDataBase.GetEffectValueAtLevel(skillNode.TargetLevel - 1)}</color> -> " +
                $"<color=#{currentHexaColor}>{skillNode.SkillDataBase.GetEffectValueAtLevel(skillNode.TargetLevel)}</color>";

        var levelReq = skillNode.SkillDataBase.GetRequirementsForLevel(skillNode.TargetLevel);
        if (costText && levelReq != null)
        {
            costText.text = $"<color=#{currentHexaColor}>{skillNode.SkillDataBase.GetCostForLevel(skillNode.TargetLevel)} $</color>";
        }

        backgroundImage.color = currentColorBackground;
        backgroundTitleImage.color = currentColorBackgroundDarker;
    }

    private void UpdateCurrentColor()
    {
        switch (skillNode.CurrentState)
        {
            case SkillNode.SkillState.Locked:
                currentHexaColor = valueHexaColorLocked;
                currentColorBackground = skillNode.TreeManager.SkillTreeVisualData.LockedColor;
                currentColorBackgroundDarker = skillNode.TreeManager.SkillTreeVisualData.LockedColorDarker;
                break;
            case SkillNode.SkillState.Available:
                currentHexaColor = valueHexaColorAvailable;
                currentColorBackground = skillNode.TreeManager.SkillTreeVisualData.AvailableColor;
                currentColorBackgroundDarker = skillNode.TreeManager.SkillTreeVisualData.AvailableColorDarker;
                break;
            case SkillNode.SkillState.Unlocked:
                currentHexaColor = valueHexaUnlocked;
                currentColorBackground = skillNode.TreeManager.SkillTreeVisualData.UnlockedColor;
                currentColorBackgroundDarker = skillNode.TreeManager.SkillTreeVisualData.UnlockedColorDarker;
                break;
            default:
                break;
        }
    }
}
