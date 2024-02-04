using System;
using System.Collections.Generic;

[System.Serializable]
public class OfferSave
{
    public List<OfferItemSave> Items = new List<OfferItemSave>();
    public void Add(OfferItemSave item)
    {
        Items.Add(item);
    }

    public OfferItemSave GetItem(int key)
    {
        return Items.Find(t => t.Id == key);
    }
}

[System.Serializable]
public class OfferItemSave
{
    public int Id;
    public bool IsBoughtFirstTime;
    public int BoughtCount = 0;

    public void Bought()
    {
        BoughtCount++;
        IsBoughtFirstTime = true;
        DataManager.Save.Offer.Save();
    }
}