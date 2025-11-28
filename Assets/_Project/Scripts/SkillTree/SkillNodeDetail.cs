using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillNodeDetail : MonoBehaviour
{
    [SerializeField] private SkillNode skillNode;

    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image backgroundTitleImage;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI statValueDescriptionText;

    private string valueHexaColorLocked;
    private string valueHexaColorAvailable;
    private string valueHexaUnlocked;

    private string currentHexaColor;
    private Color currentColorBackground;
    private Color currentColorBackgroundDarker;

    private void Start()
    {
        Initialize();
    }
    public void Initialize()
    {
        valueHexaColorAvailable = ColorUtility.ToHtmlStringRGB(skillNode.TreeManager.AvailableColorDarker);
        valueHexaUnlocked = ColorUtility.ToHtmlStringRGB(skillNode.TreeManager.UnlockedColorDarker);
        valueHexaColorLocked = ColorUtility.ToHtmlStringRGB(skillNode.TreeManager.LockedColorDarker);

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
                currentColorBackground = skillNode.TreeManager.LockedColor;
                currentColorBackgroundDarker = skillNode.TreeManager.LockedColorDarker;
                break;
            case SkillNode.SkillState.Available:
                currentHexaColor = valueHexaColorAvailable;
                currentColorBackground = skillNode.TreeManager.AvailableColor;
                currentColorBackgroundDarker = skillNode.TreeManager.AvailableColorDarker;
                break;
            case SkillNode.SkillState.Unlocked:
                currentHexaColor = valueHexaUnlocked;
                currentColorBackground = skillNode.TreeManager.UnlockedColor;
                currentColorBackgroundDarker = skillNode.TreeManager.UnlockedColorDarker;
                break;
            default:
                break;
        }
    }
}
