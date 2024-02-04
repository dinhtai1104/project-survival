using BansheeGz.BGDatabase;

[System.Serializable]
public class PlayGiftTable : DataTable<int, PlayGiftEntity>
{
    public override void GetDatabase()
    {
        DB_PlayGift.ForEachEntity(e => Get(e));
    }
    private void Get(BGEntity e)
    {
        var entity = new PlayGiftEntity(e);

        if (Dictionary.ContainsKey(entity.Id)) return;
        Dictionary.Add(entity.Id, entity);
    }
}
