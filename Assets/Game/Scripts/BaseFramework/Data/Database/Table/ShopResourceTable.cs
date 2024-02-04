using BansheeGz.BGDatabase;

[System.Serializable]
public class ShopResourceTable : DataTable<EResource, ShopResourceEntity>
{
    public override void GetDatabase()
    {
        DB_ShopResource.ForEachEntity(e => Get(e));
    }
    private void Get(BGEntity e)
    {
        var entity = new ShopResourceEntity(e);
        if (Dictionary.ContainsKey(entity.Resource)) return;
        Dictionary.Add(entity.Resource, entity);
    }
}