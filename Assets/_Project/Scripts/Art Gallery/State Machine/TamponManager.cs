using System;
using System.Collections.Generic;
using inkolorgames;
using PaintCore;
using PaintIn2D;
using UnityEngine;

public class TamponManager : MonoSingleton<TamponManager>
{
    [Header("References")]
    [SerializeField] private TamponUnlockManager unlockManager;
    [SerializeField] private CwPaintDecal2D paintDecal2D;

    [Header("Default Brush")]
    [SerializeField] private TamponData defaultTampon;
    [SerializeField, Range(0f, 100f)] private float defaultSize;
    [SerializeField, Range(0f, 100f)] private float minSize;
    [SerializeField, Range(0f, 100f)] private float maxSize;

    public static event Action<TamponData> OnTamponChanged;
    public static event Action<Texture> OnTamponTextureChanged;
    public static event Action<float> OnTamponSizeChanged;
    public static event Action<float> OnTamponOpacityChanged;

    private float currentSize = 10f;
    private float currentOpacity = 1f;
    private TamponData currentTampon;
    public TamponData CurrentTampon => currentTampon;
    public TamponData DefaultTampon => defaultTampon;
    public float MinSize => minSize;
    public float MaxSize => maxSize;
    public float CurrentSize => currentSize;
    public float CurrentOpacity => currentOpacity;

    public void EnableTamponPainting() => paintDecal2D.gameObject.SetActive(true);
    public void DisableTamponPainting() => paintDecal2D.gameObject.SetActive(false);

    protected override void Awake()
    {
        base.Awake();
        SetSize(defaultSize, false);
        ArtGalleryColorManager.OnColorChanged += SetColor;
        ArtGalleryColorManager.OnApplyColorRandomModifier += SetColorModifier;
        ArtGalleryColorManager.OnRemoveColorRandomModifier += RemoveColorModifier;
    }

    private void OnDestroy()
    {
        ArtGalleryColorManager.OnColorChanged -= SetColor;
        ArtGalleryColorManager.OnApplyColorRandomModifier -= SetColorModifier;
        ArtGalleryColorManager.OnRemoveColorRandomModifier -= RemoveColorModifier;
    }

    private void Start()
    {
        LoadTamponData();

        if (currentTampon == null)
        {
            SetDefaultTampon();
        }
    }

    private void LoadTamponData()
    {
        TamponSaveData saveData = GameSaveManager.Instance.LoadTamponData();

        if (defaultTampon != null && !unlockManager.IsItemUnlocked(defaultTampon.ItemId))
        {
            unlockManager.UnlockItem(defaultTampon.ItemId);
        }

        if (!string.IsNullOrEmpty(saveData.currentTamponId))
        {
            TamponData savedTampon = unlockManager.GetItemById(saveData.currentTamponId);
            if (savedTampon != null && unlockManager.IsItemUnlocked(savedTampon.ItemId))
            {
                SetTampon(savedTampon, false);
            }
        }
    }

    private void SetDefaultTampon()
    {
        if (defaultTampon != null)
        {
            SetTampon(defaultTampon);
        }
        else
        {
            var unlockedTampons = unlockManager.GetUnlockedItems();
            if (unlockedTampons.Count > 0)
            {
                SetTampon(unlockedTampons[0]);
            }
        }
    }

    public void SetTampon(TamponData tampon, bool saveToFile = true)
    {
        if (tampon == null)
        {
            Debug.LogError("Cannot set null tampon!");
            return;
        }

        if (!unlockManager.IsItemUnlocked(tampon.ItemId))
        {
            Debug.LogWarning($"Tampon '{tampon.ItemName}' is locked!");
            return;
        }

        currentTampon = tampon;
        SetTexture(tampon.TamponTexture, true);


        if (saveToFile)
        {
            TamponSaveData saveData = GameSaveManager.Instance.LoadTamponData();
            saveData.currentTamponId = tampon.ItemId;
            GameSaveManager.Instance.SaveTamponData(saveData);
        }

        OnTamponChanged?.Invoke(currentTampon);
        Debug.Log($"Tampon changed to: {tampon.ItemName}");
    }

    public void SetTamponById(string tamponId)
    {
        TamponData tampon = unlockManager.GetItemById(tamponId);
        if (tampon != null)
        {
            SetTampon(tampon);
        }
        else
        {
            Debug.LogError($"Tampon with ID '{tamponId}' not found!");
        }
    }

    public void SetTexture(Texture texture, bool notifyListeners = true)
    {
        paintDecal2D.Texture = texture;

        if (notifyListeners)
            OnTamponTextureChanged?.Invoke(texture);
    }

    public void SetSize(float size, bool notifyListeners = true)
    {
        currentSize = Mathf.Clamp(size, minSize, maxSize);
        paintDecal2D.Scale = Vector3.one * currentSize;

        if (notifyListeners)
            OnTamponSizeChanged?.Invoke(currentSize);
    }

    public void SetColor(Color color)
    {
        paintDecal2D.Color = color;
    }

    public void SetColorModifier(CwModifyColorRandom colorModifier = null)
    {
        paintDecal2D.Modifiers.Instances.Add(colorModifier);
    }

    public void RemoveColorModifier(CwModifyColorRandom colorModifier)
    {
        paintDecal2D.Modifiers.Instances.Remove(colorModifier);
    }

    public void SetOpacity(float opacity, bool notifyListeners = true)
    {
        currentOpacity = Mathf.Clamp01(opacity);
        paintDecal2D.Opacity = opacity;

        if (notifyListeners)
            OnTamponOpacityChanged?.Invoke(currentOpacity);
    }

    public bool IsTamponUnlocked(string tamponId) => unlockManager.IsItemUnlocked(tamponId);
    public bool IsTamponUnlocked(TamponData tampon) => tampon != null && IsTamponUnlocked(tampon.ItemId);

    public List<TamponData> GetUnlockedTampons() => unlockManager.GetUnlockedItems();
    public List<TamponData> GetLockedTampons() => unlockManager.GetLockedItems();
    public List<Unlockable<TamponData>> GetAllUnlockableTampons() => unlockManager.GetAllUnlockables();

    public string GetUnlockDescription(TamponData tampon)
    {
        return unlockManager.GetUnlockDescription(tampon);
    }

    public Sprite GetCurrentTamponSprite() => currentTampon?.TamponSprite;
}
