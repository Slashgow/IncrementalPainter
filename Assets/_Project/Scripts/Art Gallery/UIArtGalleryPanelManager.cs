using System.Collections.Generic;
using inkolorgames;
using UnityEngine;

public class UIArtGalleryPanelManager : MonoSingleton<UIArtGalleryPanelManager>
{
    [SerializeField] private GameObject brushPanel, colorPanel;
    [SerializeField] private GameObject brushSideBar, erasureSideBar;

    private GameObject currentActivePanel;

    [SerializeField] private List<GameObject> allPanels;
    [SerializeField] private List<GameObject> allSideBars;

    private void ShowPanel(GameObject panel)
    {
        if (currentActivePanel != null && currentActivePanel == panel)
        {
            HidePanel(panel);
            return;
        }
        HideAllExceptThis(panel);
        panel.SetActive(true);
        currentActivePanel = panel;
    }

    private void HideAllExceptThis(GameObject panelToShow)
    {
        foreach (GameObject panel in allPanels)
        {
            if(panel == panelToShow)
                continue;

            panel.SetActive(false);
        }
    }
    private void HidePanel(GameObject panel)
    {
        panel.SetActive(false);
        currentActivePanel = null;
    }

    public void HideAllPanel()
    {
        currentActivePanel = null;
        foreach (GameObject panel in allPanels)
        {
            panel.SetActive(false);
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
    //public void HideBrushSideBar() => brushSideBar.SetActive(false);
    public void ShowEraserSideBar() => ShowSideBar(erasureSideBar);
    //public void HideEraserSideBar() => erasureSideBar.SetActive(false);
}
