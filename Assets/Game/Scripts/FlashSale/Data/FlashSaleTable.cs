using BansheeGz.BGDatabase;
using System;
using System.Linq;

[System.Serializable]
public class FlashSaleTable : DataTable<int, FlashSaleEntity>
{
    public override void GetDatabase()
    {
        DB_FlashSale.ForEachEntity(e => Get(e));
    }

    public FlashSaleEntity Get(EFlashSale sale)
    {
        var value = Dictionary.Where(t => t.Value.Type == sale).ToList();
        if (value.Count > 0 )
        {
            return value[0].Value;
        }
        return null;
    }

    private void Get(BGEntity e)
    {
        var model = new FlashSaleEntity(e);
        if (Dictionary.ContainsKey(model.Id)) return;
        Dictionary.Add(model.Id, model);
    }
}
