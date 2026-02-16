using System;
using System.Collections.Generic;
using inkolorgames;
using PaintCore;
using UnityEngine;
using UnityEngine.InputSystem;

public class CommandHistory : MonoSingleton<CommandHistory>
{
    [SerializeField] private CwPaintableTexture paintableTexture;

    private Stack<ICommand> undoStack = new Stack<ICommand>();
    private Stack<ICommand> redoStack = new Stack<ICommand>();

    [SerializeField] private int maxHistorySize = 50;

    public bool CanUndo => undoStack.Count > 0;
    public bool CanRedo => redoStack.Count > 0;

    public static event Action OnExecutedCommand;

    private void OnEnable()
    {
        ArtGaleryInput.OnUndoPerformed += Undo;
        ArtGaleryInput.OnRedoPerformed += Redo;
        CwPaintableTexture.OnAddCommandGlobal += RecordPaintingCommand;
    }

    private void OnDisable()
    {
        ArtGaleryInput.OnUndoPerformed -= Undo;
        ArtGaleryInput.OnRedoPerformed -= Redo;
        CwPaintableTexture.OnAddCommandGlobal -= RecordPaintingCommand;
    }
    private void RecordPaintingCommand(CwPaintableTexture texture, CwCommand command)
    {
        if (command.Preview)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Debug.Log("Recording painting command: ");
            RegisterCommandWithoutExecuting(new PaintCommandWrapper());
        }
    }

    public void ExecuteCommand(ICommand command)
    {
        command.Execute();
        undoStack.Push(command);

        // Clear redo stack when a new command is executed
        redoStack.Clear();

        if (undoStack.Count > maxHistorySize)
        {
            TrimUndoStack();
        }

        OnExecutedCommand?.Invoke();
    }

    public void RegisterCommandWithoutExecuting(ICommand command)
    {
        undoStack.Push(command);
    
        redoStack.Clear();
        if (undoStack.Count > maxHistorySize)
        {
            TrimUndoStack();
        }
        OnExecutedCommand?.Invoke();
    }

    public void Undo()
    {
        if (!CanUndo)
        {
            Debug.Log("Nothing to undo");
            return;
        }

        //Debug.Log("Undoing command: " + undoStack.Peek().GetType().Name);
        ICommand command = undoStack.Pop();
        command.Undo();
        redoStack.Push(command);
        OnExecutedCommand?.Invoke();
    }


    public void Redo()
    {
        if (!CanRedo)
        {
            Debug.Log("Nothing to redo");
            return;
        }

        //Debug.Log("Redoing command: " + redoStack.Peek().GetType().Name);
        ICommand command = redoStack.Pop();
        command.Execute();
        undoStack.Push(command);
        OnExecutedCommand?.Invoke();
    }

    public void Clear()
    {
        undoStack.Clear();
        redoStack.Clear();
    }

    private void TrimUndoStack()
    {
        List<ICommand> tempList = new List<ICommand>(undoStack);
        tempList.RemoveAt(tempList.Count - 1);
        undoStack = new Stack<ICommand>(tempList);
    }

    public int GetUndoCount() => undoStack.Count;
    public int GetRedoCount() => redoStack.Count;
}