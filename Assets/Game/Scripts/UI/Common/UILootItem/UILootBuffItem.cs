using Assets.Game.Scripts.Utilities;

public class UILootBuffItem : UILootItemBase
{
    public override void SetData(ILootData lootData)
    {
        data = lootData;
        var buffData = data as BuffData;
        var entity = DataManager.Base.Buff.Get(buffData.BuffType);

        SetSprite(ResourcesLoader.Instance.GetSprite(AtlasName.Buff, entity.Icon));
    }
}