[System.Serializable]
public class DungeonEventGoldTable : DungeonTable
{
    public override void GetDatabase()
    {
        DB_DungeonEventGold.ForEachEntity(e => Get(e));
    }
}