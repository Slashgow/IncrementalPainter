using UnityEngine;

public class PaintingState : ToolState
{
    public PaintingState(ToolStateMachine stateMachine) : base(stateMachine)
    {
    }
    public override ToolType ToolType => ToolType.Painting;
    public override void EnterState()
    {
        base.EnterState();
        UIArtGalleryPanelManager.Instance.ShowPaintingPanel();
        UIArtGalleryPanelManager.Instance.HideAllSideBar();
        Draggable.EnableDrag();
    }
    public override void UpdateState()
    {
        // Handle painting logic
    }
    public override void ExitState()
    {
        base.ExitState();
        Draggable.DisableDrag();
        var transformControllers = GameObject.FindObjectsByType<TransformController>(FindObjectsSortMode.None);
        foreach (var transformController in transformControllers)
        {
            transformController.SetGizmosVisible(false);
        }
    }
}
