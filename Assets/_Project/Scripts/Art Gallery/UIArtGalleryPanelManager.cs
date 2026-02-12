using inkolorgames;
using UnityEngine;

public class UIArtGalleryPanelManager : MonoSingleton<UIArtGalleryPanelManager>
{
    [SerializeField] private GameObject brushPanel, erasurePanel;

    private GameObject currentActivePanel;

    private void ShowPanel(GameObject panel)
    {
        if (currentActivePanel != null && currentActivePanel == panel)
        {
            HidePanel(panel);
            return;
        }

        brushPanel.SetActive(true);
        currentActivePanel = panel;
    }

    private void HidePanel(GameObject panel)
    {
        panel.SetActive(false);
        currentActivePanel = null;
    }

    public void ShowBrushPanel() => ShowPanel(brushPanel);
}
