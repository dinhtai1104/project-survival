using Assets.Game.Scripts.Utilities;

public class UILootExpItem : UILootItemBase
{
    public override void SetData(ILootData lootData)
    {
        var exp = lootData as ExpData;
        data = exp;
        SetSprite(ResourcesLoader.Instance.GetSprite(AtlasName.Resources, "Exp"));
        SetAmount(exp.Exp);
    }
}
