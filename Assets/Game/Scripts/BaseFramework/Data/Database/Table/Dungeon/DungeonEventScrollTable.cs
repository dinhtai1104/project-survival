[System.Serializable]
public class DungeonEventScrollTable : DungeonTable
{
    public override void GetDatabase()
    {
        DB_DungeonEventScroll.ForEachEntity(e => Get(e));
    }
}