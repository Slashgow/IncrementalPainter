public class BrushState : ToolState
{
    public BrushState(ToolStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override ToolType ToolType => ToolType.Brush;

    public override void EnterState()
    {
        base.EnterState();
        // Initialize brush-specific settings (size, opacity, etc.)
    }

    public override void UpdateState()
    {
        // Handle brush drawing logic
    }
}
