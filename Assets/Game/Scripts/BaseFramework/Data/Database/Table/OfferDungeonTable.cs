[System.Serializable]
public class OfferDungeonTable : DataTable<int, OfferDungeonEntity>
{
    public override void GetDatabase()
    {
        DB_OfferDungeon.ForEachEntity(e =>
        {
            var package = new OfferDungeonEntity(e);
            Dictionary.Add(package.Id, package);
        });
    }
}