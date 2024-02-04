using Assets.Game.Scripts.Utilities;

public class UILootResourceItem : UILootItemBase
{
    public override void SetData(ILootData lootData)
    {
        this.data = lootData;
        var data = lootData as ResourceData;
        var sprite = ResourcesLoader.Instance.GetSprite(AtlasName.Resources, $"{data.Resource}");
        var amount = data.Value;

        SetSprite(sprite);
        SetAmount((long)amount);
    }
}
