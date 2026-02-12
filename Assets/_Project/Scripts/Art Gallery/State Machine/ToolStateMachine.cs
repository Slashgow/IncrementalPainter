using System;

public class ToolStateMachine
{
    private ToolState currentState;
    private BrushState brushState;
    private EraserState eraserState;
    private TamponState tamponState;
    private ColorSelectorState colorSelectorState;

    public event Action<ToolType> OnToolChanged;
    public ToolState CurrentState => currentState;

    public ToolStateMachine()
    {
        brushState = new BrushState(this);
        eraserState = new EraserState(this);
        tamponState = new TamponState(this);
        colorSelectorState = new ColorSelectorState(this);

        TransitionToState(brushState);
    }

    public void TransitionToState(ToolState newState)
    {
        currentState?.ExitState();
        currentState = newState;
        currentState.EnterState();
        OnToolChanged?.Invoke(currentState.ToolType);
    }

    public void TransitionToTool(ToolType toolType)
    {
        ToolState newState = toolType switch
        {
            ToolType.Brush => brushState,
            ToolType.Eraser => eraserState,
            ToolType.Tampon => tamponState,
            ToolType.ColorSelector => colorSelectorState,
            _ => brushState
        };

        TransitionToState(newState);
    }

    public void Update()
    {
        currentState?.UpdateState();
    }
}
