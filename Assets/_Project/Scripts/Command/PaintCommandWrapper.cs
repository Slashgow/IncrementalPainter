using PaintCore;

public class PaintCommandWrapper : ICommand
{
    public void Execute() => CwStateManager.RedoAll();
    public void Undo() => CwStateManager.UndoAll();
}
