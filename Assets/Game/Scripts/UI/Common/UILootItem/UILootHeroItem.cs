using Assets.Game.Scripts.Utilities;

public class UILootHeroItem : UILootItemBase
{
    public override void SetData(ILootData lootData)
    {
        var hero = lootData as HeroData;
        if (hero == null) return;
        data = hero;

        var spriteHero = ResourcesLoader.Instance.GetSprite(AtlasName.Hero, $"{hero.HeroType}");
        SetSprite(spriteHero);
    }
}