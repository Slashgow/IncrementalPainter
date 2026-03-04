using inkolorgames;
using UnityEngine;

public abstract class PopupSpawner<TItem, TManager> : MonoBehaviour
    where TItem : ScriptableObject, IUnlockableItem
    where TManager : MonoSingleton<TManager>
{
    [SerializeField] private UnlockManager<TItem, TManager> unlockManager;
    [SerializeField] protected PopUp popupPrefab;
    [SerializeField] protected Canvas popupParent;

    private void Awake() => popupParent.gameObject.SetActive(false);
    private void OnEnable() => unlockManager.OnItemUnlocked += OnItemUnlocked;
    private void OnDisable() => unlockManager.OnItemUnlocked -= OnItemUnlocked;
    protected abstract void OnItemUnlocked(string id);
}
