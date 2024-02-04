[System.Serializable]
public class ExpRequireTable : DataTable<int, long>
{
    public override void GetDatabase()
    {
        DB_Exp.ForEachEntity(e =>
        {
            int level = e.Get<int>("Level");
            long expRequire = e.Get<long>("ExpRequire");

            Dictionary.Add(level, expRequire);
        });
    }
}