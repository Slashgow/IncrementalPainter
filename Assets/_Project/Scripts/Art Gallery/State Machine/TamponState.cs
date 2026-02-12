public class TamponState : ToolState
{
    public TamponState(ToolStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override ToolType ToolType => ToolType.Tampon;

    public override void EnterState()
    {
        base.EnterState();
        // Initialize tampon/stamp tool settings
    }

    public override void UpdateState()
    {
        // Handle tampon/stamp logic
    }
}
