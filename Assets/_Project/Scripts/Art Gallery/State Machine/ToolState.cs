using System;
using UnityEngine;

public abstract class ToolState
{
    protected ToolStateMachine stateMachine;

    protected ToolState(ToolStateMachine stateMachine) => this.stateMachine = stateMachine;

    public abstract ToolType ToolType { get; }

    public virtual void EnterState()
    {
        Debug.Log($"Entered {ToolType} state");
    }

    public virtual void ExitState()
    {
        Debug.Log($"Exited {ToolType} state");
    }

    public virtual void UpdateState() { }

}
