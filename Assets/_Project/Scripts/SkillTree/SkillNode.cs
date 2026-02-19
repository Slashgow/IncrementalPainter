using System.Collections.Generic;
using Coffee.UIEffects;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillNode : MonoBehaviour, IColorChanger
{
    [Header("References")]
    [SerializeField] private PointerEventRegistered pointerEventRegistered;
    [SerializeField] private SkillDataBase skillDataBase;
    public SkillDataBase SkillDataBase => skillDataBase;

    [SerializeField] private Image iconImage;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Button button;

    [Header("Connection Lines")]
    [SerializeField] private List<LineRenderer> connectionLines;
    public List<LineRenderer> ConnectionLines => connectionLines;

    private SkillTreeManager treeManager;
    public SkillTreeManager TreeManager => treeManager;

    private SkillState currentState;
    public SkillState CurrentState => currentState;

    private SkillNodeDetail activeDetailInstance;

    [SerializeField] private UnityEvent OnDiscoverNode;

    private bool isDiscovered;
    public bool IsDiscovered => isDiscovered;

    private UIEffect uiEffect;

    public enum SkillState
    {
        Locked,
        Available,
        Unlocked
    }

    private void Awake()
    {
        treeManager = GetComponentInParent<SkillTreeManager>();
        uiEffect = button.GetComponent<UIEffect>();
    }
    private void Start()
    {
        Initialize();
    }

    private void OnEnable()
    {
        ThemeColorManager.OnThemeChanged += OnThemeChanged;
        pointerEventRegistered.OnPointerEnterEvent += OnPointerEnter;
        pointerEventRegistered.OnPointerExitEvent += OnPointerExit;
        pointerEventRegistered.OnPointerClickEvent += OnPointerClick;
        button.onClick.AddListener(OnSkillClicked);
    }

    private void OnDisable()
    {
        ThemeColorManager.OnThemeChanged -= OnThemeChanged;
        pointerEventRegistered.OnPointerEnterEvent -= OnPointerEnter;
        pointerEventRegistered.OnPointerExitEvent -= OnPointerExit;
        pointerEventRegistered.OnPointerClickEvent -= OnPointerClick;
        button.onClick.RemoveListener(OnSkillClicked);
    }

    public void OnThemeChanged(ColorTheme newTheme) => UpdateColor();
    public void UpdateColor() => UpdateVisuals();
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isDiscovered)
            return;

        if (treeManager.DetailPrefab != null && treeManager.DetailCanvas != null)
        {
            //activeDetailInstance = Instantiate(treeManager.DetailPrefab, 
            //    (Vector2)treeManager.GetScreenPosition(this.transform.position) + treeManager.DetailOffset, 
            //    Quaternion.identity, 
            //    treeManager.DetailCanvas);
            //activeDetailInstance.Initialize(this);

            activeDetailInstance = Instantiate(treeManager.DetailPrefab,Vector3.zero,Quaternion.identity,treeManager.DetailCanvas.transform);

            Vector2 safePos = Helper.GetSafeUIPosition(
                this.transform.position,
                activeDetailInstance.GetComponent<RectTransform>(),
                treeManager.DetailCanvas,
                treeManager.DetailOffset,
                avoidanceRadius: 40f);          // tune to roughly match the object's screen size

            activeDetailInstance.GetComponent<RectTransform>().anchoredPosition = safePos;
            activeDetailInstance.Initialize(this);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (activeDetailInstance != null)
        {
            Destroy(activeDetailInstance.gameObject);
            activeDetailInstance = null;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isDiscovered)
            return;

        //if (eventData.button == PointerEventData.InputButton.Left)
        //{
        //    OnSkillClicked();
        //}
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnSkillRightClicked();
        }
    }
    private void OnSkillRightClicked()
    {
        if (treeManager != null)
            treeManager.TryRefundLastLevel(skillDataBase);
    }
    public void Initialize()
    {
        if (iconImage) 
            iconImage.sprite = skillDataBase.Icon;

        UpdateVisuals();
    }

    void OnSkillClicked()
    {
        if (treeManager != null)
        {
            treeManager.TryLevelUpSkill(skillDataBase);
        }
    }

    public void UpdateVisuals()
    {
        if (skillDataBase == null)
            return;

        bool wasDiscovered = isDiscovered;
        isDiscovered = CheckIfDiscovered();

        if (!wasDiscovered && isDiscovered)
            OnDiscoverNode?.Invoke();

        SetNodeVisibility(isDiscovered);

        if (!isDiscovered)
            return;

        currentState = DetermineState();

        switch (currentState)
        {
            case SkillState.Locked:
                if(uiEffect != null)
                    uiEffect.enabled = false;
                if (backgroundImage) 
                    backgroundImage.sprite = treeManager.SkillTreeVisualData.BackgroundSpriteLocked;
                    backgroundImage.color = treeManager.SkillTreeVisualData.LockedColor;
                if (button) 
                    button.interactable = false;
                break;

            case SkillState.Available:
                if (uiEffect != null)
                    uiEffect.enabled = true;
                if (backgroundImage) 
                    backgroundImage.color = treeManager.SkillTreeVisualData.AvailableColor;
                if (button) 
                    button.interactable = true;
                break;

            case SkillState.Unlocked:
                if (uiEffect != null)
                    uiEffect.enabled = false;
                if (backgroundImage)
                    backgroundImage.sprite = treeManager.SkillTreeVisualData.BackgroundSpriteUnlocked;
                backgroundImage.color = treeManager.SkillTreeVisualData.UnlockedColor;
                if (button) 
                    button.interactable = false;
                break;
        }

        UpdateConnectionLines();
    }
    private bool CheckIfDiscovered()
    {
        var levelRequirement = skillDataBase.GetRequirementsForLevel(1);

        if (levelRequirement == null || levelRequirement.RequiredSkills == null || levelRequirement.RequiredSkills.Count == 0)
            return true;

        foreach (var requiredSkillWithLevel in levelRequirement.RequiredSkills)
        {
            if (requiredSkillWithLevel.SkillData == null)
                continue;

            int currentLevel = treeManager.GetSkillLevel(requiredSkillWithLevel.SkillData.SkillID);
            if (currentLevel < 1)
                return false;
        }

        return true;
    }

    private void SetNodeVisibility(bool visible)
    {
        if (iconImage)
            iconImage.gameObject.SetActive(visible);
        if (backgroundImage)
            backgroundImage.gameObject.SetActive(visible);
        if (button)
            button.gameObject.SetActive(visible);

        foreach (var line in connectionLines)
        {
            if (line != null)
                line.gameObject.SetActive(visible);
        }
    }

    private SkillState DetermineState()
    {
        int currentLevel = treeManager.GetSkillLevel(skillDataBase.SkillID);

        if (currentLevel >= skillDataBase.MaxLevel)
            return SkillState.Unlocked;

        if (treeManager.CanLevelUpToLevel(skillDataBase, currentLevel + 1))
            return SkillState.Available;

        return SkillState.Locked;
    }

    void UpdateConnectionLines()
    {
        foreach (var line in connectionLines)
        {
            if (line != null)
            {
                Color lineColor = currentState == SkillState.Unlocked ? treeManager.SkillTreeVisualData.UnlockedColorDarker : 
                    treeManager.SkillTreeVisualData.LockedColorDarker;
                line.startColor = lineColor;
                line.endColor = lineColor;
            }
        }
    }

#if UNITY_EDITOR
    public void CreateConnectionLine(SkillNode targetNode)
    {
        GameObject lineObj = new GameObject($"Line_{skillDataBase.SkillName}_to_{targetNode.skillDataBase.SkillName}");
        lineObj.transform.SetParent(transform.parent);

        LineRenderer line = lineObj.AddComponent<LineRenderer>();
        line.positionCount = 2;
        line.startWidth = 0.2f;
        line.endWidth = 0.2f;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.sortingOrder = -1;

        line.SetPosition(0, transform.position);
        line.SetPosition(1, targetNode.transform.position);

        connectionLines.Add(line);
    }

    [ContextMenu("Create Connections to Required Skills")]
    public void CreateConnectionsToRequiredSkills()
    {
        ClearConnectionLines();

        if (treeManager == null)
            treeManager = GetComponentInParent<SkillTreeManager>();

        if (treeManager == null)
        {
            Debug.LogError("SkillTreeManager not found!");
            return;
        }

        HashSet<SkillDataBase> uniqueRequiredSkills = new HashSet<SkillDataBase>();

        foreach (var levelReq in skillDataBase.LevelRequirements)
        {
            if (levelReq != null && levelReq.RequiredSkills != null)
            {
                foreach (var requiredSkillWithLevel in levelReq.RequiredSkills)
                {
                    if (requiredSkillWithLevel.SkillData != null)
                    {
                        uniqueRequiredSkills.Add(requiredSkillWithLevel.SkillData);
                    }
                }
            }
        }

        foreach (var requiredSkill in uniqueRequiredSkills)
        {
            SkillNode requiredNode = treeManager.FindNodeBySkillData(requiredSkill);
            if (requiredNode != null)
            {
                CreateConnectionLine(requiredNode);
                Debug.Log($"Connected {skillDataBase.SkillName} to required skill: {requiredSkill.SkillName}");
            }
            else
            {
                Debug.LogWarning($"Could not find node for required skill: {requiredSkill.SkillName}");
            }
        }

        if (connectionLines.Count == 0)
        {
            Debug.Log($"{skillDataBase?.SkillName ?? "Skill"} has no connections to create");
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
