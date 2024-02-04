using System.Collections.Generic;

[System.Serializable]
public class OffersSave : BaseDatasave
{
    public OfferSave GemSave;
    public OfferSave GoldSave;
    public OfferSave OfferDungeon;

    public OffersSave(string key) : base(key)
    {
        GemSave = new OfferSave();
        GoldSave = new OfferSave();
        OfferDungeon = new OfferSave();

        var gemTable = DataManager.Base.OfferGem;
        foreach (var data in gemTable.Dictionary)
        {
            var id = data.Key;
            GemSave.Add(new OfferItemSave { Id = id, IsBoughtFirstTime = false });
        } 
        
        var goldTable = DataManager.Base.OfferGold;
        foreach (var data in goldTable.Dictionary)
        {
            var id = data.Key;
            GoldSave.Add(new OfferItemSave { Id = id, IsBoughtFirstTime = false });
        }
        
        var dungeonTable = DataManager.Base.OfferDungeon;
        foreach (var data in dungeonTable.Dictionary)
        {
            var id = data.Key;
            OfferDungeon.Add(new OfferItemSave { Id = id, IsBoughtFirstTime = false });
        }
    }

    public override void Fix()
    {
        if (GemSave == null)
        {
            GemSave = new OfferSave();
        }
        var gemTable = DataManager.Base.OfferGem;
        foreach (var data in gemTable.Dictionary)
        {
            if (GemSave.Items.Find(t => t.Id == data.Key) == null)
            {
                var id = data.Key;
                GemSave.Add(new OfferItemSave { Id = id, IsBoughtFirstTime = false });
            }
        }
        if (GoldSave == null)
        {
            GoldSave = new OfferSave();
        }
        var goldTable = DataManager.Base.OfferGold;
        foreach (var data in goldTable.Dictionary)
        {
            if (GoldSave.Items.Find(t => t.Id == data.Key) == null)
            {
                var id = data.Key;
                GoldSave.Add(new OfferItemSave { Id = id, IsBoughtFirstTime = false });
            }
        }
        if (OfferDungeon == null)
        {
            OfferDungeon = new OfferSave();
        }
        var dungeonTable = DataManager.Base.OfferDungeon;
        foreach (var data in dungeonTable.Dictionary)
        {
            if (OfferDungeon.Items.Find(t => t.Id == data.Key) == null)
            {
                var id = data.Key;
                OfferDungeon.Add(new OfferItemSave { Id = id, IsBoughtFirstTime = false });
            }
        }
    }
}
