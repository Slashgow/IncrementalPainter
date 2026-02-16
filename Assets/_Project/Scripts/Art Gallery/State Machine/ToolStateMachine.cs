using System;

public class ToolStateMachine
{
    private static ToolState currentState;
    private BrushState brushState;
    private EraserState eraserState;
    private TamponState tamponState;
    private ColorSelectorState colorSelectorState;
    private PaintingState paintingState;

    public event Action<ToolType> OnToolChanged;
    public static ToolState CurrentState => currentState;

    public ToolStateMachine()
    {
        brushState = new BrushState(this);
        eraserState = new EraserState(this);
        tamponState = new TamponState(this);
        colorSelectorState = new ColorSelectorState(this);
        paintingState = new PaintingState(this);

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
            ToolType.Painting => paintingState,
            _ => brushState
        };

        TransitionToState(newState);
    }

    public void Update()
    {
        currentState?.UpdateState();
    }
}
