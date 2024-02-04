using UnityEngine;
using System.Collections.Generic;
using System;
using Sirenix.OdinInspector;
using Assets.Game.Scripts.DungeonWorld.Data;
using Assets.Game.Scripts.PiggyBank.Data;
using Assets.Game.Scripts.Talent.Database;

public class DatabaseManager : MonoSingleton<DatabaseManager>
{
    private List<DataTable> databases = new List<DataTable>();

    public GeneralConfigTable GeneralConfig = new GeneralConfigTable();
    public HeroTable Hero = new HeroTable();
    public EnemyTable Enemy = new EnemyTable();
    public HeroLevelUpgradeTable HeropLevelUpgrade = new HeroLevelUpgradeTable();
    public HeroStarUpgradeTable HeroStarUpgrade = new HeroStarUpgradeTable();
    public BuffTable Buff = new BuffTable();
    public EquipmentTable Equipment = new EquipmentTable();
    public EquipmentRarityTable EquipmentRarity = new EquipmentRarityTable();
    public ExpRequireTable ExpRequire = new ExpRequireTable();
    public WeaponTable Weapon = new WeaponTable();
    public BlackSmithUpgradeTable BlackSmithUpgrade = new BlackSmithUpgradeTable();
    public AffixEquipmentTable AffixEquipment = new AffixEquipmentTable();
    public RoomTable Room = new RoomTable();
    public DungeonStageTable DungeonStage = new DungeonStageTable();
    public DungeonTable Dungeon = new DungeonTable();
    public OfferDungeonTable OfferDungeon = new OfferDungeonTable();
    public IapConfigTable IapConfig = new IapConfigTable();
    public ChestTable Chest = new ChestTable();
    public OfferGemTable OfferGem = new OfferGemTable();
    public OfferGoldTable OfferGold = new OfferGoldTable();
    public DailyQuestTable DailyQuest = new DailyQuestTable();
    public DailyQuestProgressTable DailyQuestProgress = new DailyQuestProgressTable();
    public AchievementQuestTable AchievementQuest = new AchievementQuestTable();
    public SkillTreeTable SkillTree = new SkillTreeTable();
    public DungeonRoomRewardTable DungeonRoomReward = new DungeonRoomRewardTable();
    public HotSaleHeroTable HotSaleHero = new HotSaleHeroTable();
    public SubscriptionTable Subscription = new SubscriptionTable();
    public DungeonWorldTable DungeonWorld = new DungeonWorldTable();
    public PiggyBankTable PiggyBank = new PiggyBankTable();
    public FlashSaleTable FlashSale = new FlashSaleTable();
    public PlayGiftTable PlayGift = new PlayGiftTable();
    public ShopResourceTable ShopResource = new ShopResourceTable();
    public BattlePassTable BattlePass = new BattlePassTable();
    public TalentTable Talent = new TalentTable();

    public DungeonEventGoldTable DungeonEventGold = new DungeonEventGoldTable();
    public DungeonEventScrollTable DungeonEventScroll = new DungeonEventScrollTable();
    public DungeonEventStoneTable DungeonEventStone = new DungeonEventStoneTable();
    public DungeonEventConfigTable DungeonEventConfig = new DungeonEventConfigTable();

    [ShowInInspector]
    public Dictionary<EDungeonEvent, DungeonTable> DungeonEventTables = new Dictionary<EDungeonEvent, DungeonTable>();

    public void Init(Transform parent = null)
    {
        DataManager.Base = this;
        if (parent) transform.SetParent(parent);

        PreloadData();
        LoadDatabase();
    }

    private void PreloadData()
    {
        databases.Add(GeneralConfig);
        databases.Add(IapConfig);
        databases.Add(Hero);
        databases.Add(Enemy);
        databases.Add(HeropLevelUpgrade);
        databases.Add(HeroStarUpgrade);
        databases.Add(Equipment);
        databases.Add(EquipmentRarity);
        databases.Add(Buff);
        databases.Add(ExpRequire);
        databases.Add(Weapon);
        databases.Add(BlackSmithUpgrade);
        databases.Add(AffixEquipment);
        databases.Add(Room);
        databases.Add(DungeonStage);
        databases.Add(Dungeon);
        databases.Add(OfferDungeon);
        databases.Add(Chest);
        databases.Add(OfferGem);
        databases.Add(OfferGold);
        databases.Add(DungeonEventConfig);
        databases.Add(DailyQuest);
        databases.Add(DailyQuestProgress);
        databases.Add(AchievementQuest);
        databases.Add(SkillTree);
        databases.Add(HotSaleHero);
        databases.Add(Subscription);
        databases.Add(DungeonWorld);
        databases.Add(PiggyBank);
        databases.Add(FlashSale);
        databases.Add(PlayGift);
        databases.Add(ShopResource);
        databases.Add(BattlePass);
        databases.Add(Talent);

        DungeonEventTables.Clear();

        DungeonEventTables.Add(EDungeonEvent.Gold, DungeonEventGold);
        DungeonEventTables.Add(EDungeonEvent.Scroll, DungeonEventScroll);
        DungeonEventTables.Add(EDungeonEvent.Stone, DungeonEventStone);

        databases.Add(DungeonEventGold);
        databases.Add(DungeonEventScroll);
        databases.Add(DungeonEventStone);

        databases.Add(DungeonRoomReward);
    } 

    public void LoadDatabase()
    {
        foreach (var database in databases)
        {
            database.Clear();
            database.GetDatabase();
        }

        // refactor data upgrade entity for heroes
        foreach (var hero in this.Hero.Dictionary)
        {
            hero.Value.SetLevelUpgrades(HeropLevelUpgrade.GetUpgradesHero(hero.Key));
            hero.Value.SetStarUpgrades(HeroStarUpgrade.GetUpgradesHero(hero.Key));
        }
    }

    public void Reload()
    {
        databases.Clear();
        PreloadData();
        LoadDatabase();
    }
}