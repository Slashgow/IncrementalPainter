public class BrushState : ToolState
{
    public BrushState(ToolStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override ToolType ToolType => ToolType.Brush;

    public override void EnterState()
    {
        base.EnterState();
        UIArtGalleryPanelManager.Instance.ShowBrushPanel();
        BrushManager.Instance.EnableBrushPainting();
    }

    public override void UpdateState()
    {
        // Handle brush drawing logic
    }

    public override void ExitState()
    {
        base.ExitState();
        BrushManager.Instance.DisableBrushPainting();
    }
}
