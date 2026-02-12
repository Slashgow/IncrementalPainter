public class EraserState : ToolState
{
    public EraserState(ToolStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override ToolType ToolType => ToolType.Eraser;

    public override void EnterState()
    {
        base.EnterState();
        // Initialize eraser-specific settings
    }

    public override void UpdateState()
    {
        // Handle eraser logic
    }
}
