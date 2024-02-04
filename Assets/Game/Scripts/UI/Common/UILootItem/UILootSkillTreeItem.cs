using Assets.Game.Scripts.Utilities;

public class UILootSkillTreeItem : UILootItemBase
{
    public override void SetData(ILootData lootData)
    {
        data = lootData;
        var statData = lootData as SkillTreeLootData;
        SetSprite(ResourcesLoader.Instance.GetSprite(AtlasName.SkillTree, statData.statKey.ToString()));
        SetText("");
    }
}
