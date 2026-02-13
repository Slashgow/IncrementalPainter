using System.Collections.Generic;
using UnityEngine;

public class UITamponLoader : MonoBehaviour
{
    [SerializeField] private Transform parent;
    [SerializeField] private UITampon tamponUIPrefab;

    private List<UITampon> uiTampons = new List<UITampon>();

    public void Start()
    {
        InitializeTamponUI();
    }

    private void InitializeTamponUI()
    {
        parent.DestroyAllChildren();

        List<TamponData> tampons = TamponManager.Instance.GetUnlockedTampons();

        foreach (TamponData tampon in tampons)
        {
            UITampon uiTamponInstance = Instantiate(tamponUIPrefab, parent);
            uiTamponInstance.Initialize(tampon);
            uiTampons.Add(uiTamponInstance);
            uiTamponInstance.OnSelectEvent += OnSelectBrush;
        }
    }

    private void OnDestroy()
    {
        foreach (UITampon uiTampon in uiTampons)
        {
            uiTampon.OnSelectEvent -= OnSelectBrush;
        }
    }

    private void OnSelectBrush(TamponData tamponData)
    {
        TamponManager.Instance.SetTampon(tamponData);
    }
}
