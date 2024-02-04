using Assets.Game.Scripts.Utilities;

public class UILootFragmentItem : UILootItemBase
{
    public override void SetData(ILootData lootData)
    {
        var data = lootData as ResourceData;
        var fragType = data.Resource;
        var icon = ResourcesLoader.Instance.GetSprite(AtlasName.Resources, $"{fragType}");
        SetSprite(icon);
        SetAmount((long)data.Value);
    }
}
    