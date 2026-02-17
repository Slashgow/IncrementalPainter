using System.Collections.Generic;
using inkolorgames;
using UnityEngine;

public class UIArtGalleryPanelManager : MonoSingleton<UIArtGalleryPanelManager>
{
    [SerializeField] private CanvasGroup brushPanel, colorPanel, tamponPanel, paintingPanel;
    [SerializeField] private GameObject brushSideBar, erasureSideBar, tamponSideBar;

    private CanvasGroup currentActivePanel;

    [SerializeField] private List<CanvasGroup> allPanels;
    [SerializeField] private List<GameObject> allSideBars;

    public void ShowPanel(CanvasGroup canvasGroup)
    {
        if (currentActivePanel != null && currentActivePanel == canvasGroup)
        {
            HidePanel(canvasGroup);
            return;
        }
        HideAllExceptThis(canvasGroup);

        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts = true;
        currentActivePanel = canvasGroup;
    }
    public void HidePanel(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        currentActivePanel = null;
    }

    private void HideAllExceptThis(CanvasGroup canvasGroup)
    {
        foreach (CanvasGroup panel in allPanels)
        {
            if(panel == canvasGroup)
                continue;

            HidePanel(panel);
        }
    }

    public void HideAllPanel()
    {
        currentActivePanel = null;
        foreach (CanvasGroup panel in allPanels)
        {
            HidePanel(panel);
        }
    }

    public void HideAllSideBar()
    {
        foreach (GameObject sb in allSideBars)
        {
            sb.SetActive(false);
        }
    }

    public void ShowSideBar(GameObject sideBar)
    {
        foreach (GameObject sb in allSideBars)
        {
            if (sb == sideBar)
                continue;
            sb.SetActive(false);
        }
        sideBar.SetActive(true); 
    }
    public void ShowBrushPanel() => ShowPanel(brushPanel);
    public void ShowBrushSideBar() => ShowSideBar(brushSideBar);
    public void ShowEraserSideBar() => ShowSideBar(erasureSideBar);
    public void ShowColorPanel() => ShowPanel(colorPanel);
    public void ShowTamponPanel() => ShowPanel(tamponPanel);
    public void ShowTamponSideBar() => ShowSideBar(tamponSideBar);
    public void ShowPaintingPanel() => ShowPanel(paintingPanel);
}
