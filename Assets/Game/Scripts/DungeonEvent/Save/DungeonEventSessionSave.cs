[System.Serializable]
public class DungeonEventSessionSave : DungeonSessionSave
{
    public EDungeonEvent Type;
    public DungeonEventSessionSave(string key) : base(key)
    {
    }

    public void SetDungeonEvent(EDungeonEvent e)
    {
        Type = e;
        Save();
    }
}