[System.Serializable]
public class DungeonEventStoneTable : DungeonTable
{
    public override void GetDatabase()
    {
        DB_DungeonEventStone.ForEachEntity(e => Get(e));
    }
}