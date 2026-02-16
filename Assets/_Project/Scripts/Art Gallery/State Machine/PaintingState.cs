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
    }
    public override void UpdateState()
    {
        // Handle painting logic
    }
    public override void ExitState()
    {
        base.ExitState();
        // Hide painting UI, disable painting mode
    }
}
