using System;
using inkolorgames;
using inkolorgames.effects;
using UnityEngine;

[Serializable]
public struct CursorData
{
    [SerializeField] private Texture2D cursorTexture;
    [SerializeField] private Vector2 hotSpot;
    [SerializeField] private CursorMode cursorMode;
    public Texture2D CursorTexture => cursorTexture;
    public Vector2 HotSpot => hotSpot;
    public CursorMode CursorMode => cursorMode;
}

public class BaseCursorHandler : PersistentMonoSingleton<BaseCursorHandler>
{
    [SerializeField] private CursorData defaultCursor;
    [SerializeField] private CursorData hoverCardCursor;
    [SerializeField] private CursorData grabCursor;

    protected virtual void OnEnable()
    {
        SetCursor(defaultCursor);

        SelectableInputRegister.OnAnySelectableExit += SelectableInputRegister_OnAnySelectableExit;
        SelectableInputRegister.OnAnySelectableHover += SelectableInputRegister_OnAnySelectableHover;
        SelectableInputRegister.OnAnySelectablePressed += SelectableInputRegister_OnAnySelectablePressed;
    }

    protected virtual void OnDisable()
    {
        SelectableInputRegister.OnAnySelectableExit -= SelectableInputRegister_OnAnySelectableExit;
        SelectableInputRegister.OnAnySelectableHover -= SelectableInputRegister_OnAnySelectableHover;
        SelectableInputRegister.OnAnySelectablePressed -= SelectableInputRegister_OnAnySelectablePressed;
    }
    public void SetCursor(CursorData cursorData) => Cursor.SetCursor(cursorData.CursorTexture, cursorData.HotSpot, cursorData.CursorMode);

    private void SelectableInputRegister_OnAnySelectableExit() => SetCursor(defaultCursor);
    private void SelectableInputRegister_OnAnySelectablePressed() => SetCursor(grabCursor);
    private void SelectableInputRegister_OnAnySelectableHover() => SetCursor(hoverCardCursor);
}

//public class InGameCursorHandler : BaseCursorHandler
//{
//    protected override void OnEnable()
//    {
//        base.OnEnable();
//        GameManager.OnStartGameState += GameManager_OnStartGameState;
//    }
//
//    override protected void OnDisable()
//    {
//        base.OnDisable();
//    }
//
//    private void GameManager_OnStartGameState(GameManager.GameState gameState)
//    {
//        throw new NotImplementedException();
//    }
//}