using System.Collections.Generic;
using UnityEngine;

public class UIBrushLoader : MonoBehaviour
{
    [SerializeField] private Transform parent;
    [SerializeField] private UIBrush brushUIPrefab;

    private List<UIBrush> uiBrushes = new List<UIBrush>();

    public void Start()
    {
        InitializeBrushUI();
    }

    private void InitializeBrushUI()
    {
        parent.DestroyAllChildren();

        List<BrushData> brushes = BrushManager.Instance.GetUnlockedBrushes();

        foreach (BrushData brush in brushes)
        {
            UIBrush uibrushInstance = Instantiate(brushUIPrefab, parent);
            uibrushInstance.Initialize(brush);
            uiBrushes.Add(uibrushInstance);
            uibrushInstance.OnSelectEvent += OnSelectBrush;
        }
    }

    private void OnDestroy()
    {
        foreach (UIBrush uiBrush in uiBrushes)
        {
            uiBrush.OnSelectEvent -= OnSelectBrush;
        }
    }

    private void OnSelectBrush(BrushData brushData)
    {
        BrushManager.Instance.SetBrush(brushData);
    }
}
