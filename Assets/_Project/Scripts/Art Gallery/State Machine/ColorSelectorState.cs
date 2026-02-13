public class ColorSelectorState : ToolState
{
    public ColorSelectorState(ToolStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override ToolType ToolType => ToolType.ColorSelector;

    public override void EnterState()
    {
        base.EnterState();
        UIArtGalleryPanelManager.Instance.ShowColorPanel();
    }

    public override void UpdateState()
    {
        // Handle color selection logic
    }
}
