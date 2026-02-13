public class TamponState : ToolState
{
    public TamponState(ToolStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override ToolType ToolType => ToolType.Tampon;

    public override void EnterState()
    {
        base.EnterState();
        UIArtGalleryPanelManager.Instance.ShowTamponPanel();
        UIArtGalleryPanelManager.Instance.ShowTamponSideBar();
        TamponManager.Instance.EnableTamponPainting();
    }

    public override void UpdateState()
    {
        // Handle tampon/stamp logic
    }

    public override void ExitState()
    {
        base.ExitState();
        TamponManager.Instance.DisableTamponPainting();
    }
}
