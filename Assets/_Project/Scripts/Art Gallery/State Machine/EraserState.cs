public class EraserState : ToolState
{
    public EraserState(ToolStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override ToolType ToolType => ToolType.Eraser;

    public override void EnterState()
    {
        base.EnterState();
        UIArtGalleryPanelManager.Instance.HideAllPanel();
        UIArtGalleryPanelManager.Instance.ShowEraserSideBar();
        ErasureManager.Instance.EnableEraser();
    }

    public override void UpdateState()
    {
        // Handle eraser logic
    }

    override public void ExitState()
    {
        base.ExitState();
        ErasureManager.Instance.DisableEraser();
    }
}
