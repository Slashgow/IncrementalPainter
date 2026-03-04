using System.Collections.Generic;

public class TamponUnlockManager : UnlockManager<TamponData, TamponUnlockManager>
{
    private const string SAVE_KEY = "Tampon";

    protected override string GetSaveKey() => SAVE_KEY;
    protected override UnlockableSaveData LoadSaveData()
    {
        TamponSaveData brushSaveData = GameSaveManager.Instance.LoadTamponData();

        UnlockableSaveData saveData = new UnlockableSaveData();
        saveData.unlockedItemIds = new List<string>(brushSaveData.unlockedTamponIds);

        return saveData;
    }

    protected override void SaveData(UnlockableSaveData saveData)
    {
        TamponSaveData tamponSaveData = new TamponSaveData();
        tamponSaveData.unlockedTamponIds = new List<string>(saveData.unlockedItemIds);

        GameSaveManager.Instance.SaveTamponData(tamponSaveData);
    }
}
