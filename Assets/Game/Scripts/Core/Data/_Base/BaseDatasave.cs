using BayatGames.SaveGameFree;
using Sirenix.OdinInspector;

[System.Serializable]
public abstract class BaseDatasave : IDatasave
{
    public string Key;
    public BaseDatasave()
    {

    }
    public BaseDatasave(string key)
    {
        this.Key = key;
    }
    [Button]
    public virtual void Save()
    {
        if (Key == "NotSave") return;
        SaveGame.Save(Key, this);
    }
    public virtual void OnLoaded() { }
    public abstract void Fix();

    public virtual void NextDay()
    {
    }
}