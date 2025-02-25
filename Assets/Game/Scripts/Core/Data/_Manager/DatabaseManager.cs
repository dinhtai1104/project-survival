using UnityEngine;
using System.Collections.Generic;
using Assets.Game.Scripts.Core.Data.Database;
using Assets.Game.Scripts.Core.Data.Database.Dungeon;
using Assets.Game.Scripts.Core.Data.Database.Equipment.Gear;
using Assets.Game.Scripts.Core.Data.Database.Equipment;
using Assets.Game.Scripts.Core.Data.Database.Equipment.Weapon;
using Assets.Game.Scripts.Core.Data.Database.Rarity;
using Assets.Game.Scripts.Core.Data.Database.Dungeon.EnemyEvent;
using Assets.Game.Scripts.Core.Data.Database.Config;
using Assets.Game.Scripts.Core.Data.Database.Buff;
using Assets.Game.Scripts.Core.Data.Database.LevelExp;
using Assets.Game.Scripts.Core.Data.Database.LevelUpBuff;
using Assets.Game.Scripts.Core.Data.Database.ShopWave;

public class DatabaseManager : MonoSingleton<DatabaseManager>
{
    private List<DataTable> databases = new();

    public GameConfigTable GameConfig = new();
    public EnemyTable EnemyTable = new();
    public DungeonTable Dungeon = new();
    public EnemiesEventTable EnemiesEvent = new();
    public DungeonWaveTable DungeonWave = new();

    public RarityTable Rarity = new();
    public PerkRarityTable GearPerk = new();
    public EquipmentTable Equipment = new();
    public WeaponTable Weapon = new();
    public BuffTable Buff = new();
    public LevelExpGameplayTable LevelExpGameplay = new();
    public LevelExpAccountTable LevelExpAccount = new();
    public LevelUpBuffTable LevelUpBuff = new();
    public ShopWaveConfigTable ShopWaveConfig = new();

    public void Init(Transform parent = null)
    {
        DataManager.Base = this;
        if (parent) transform.SetParent(parent);

        PreloadData();
        LoadDatabase();
    }

    private void PreloadData()
    {
        databases.Add(GameConfig);

        databases.Add(EnemyTable);
        databases.Add(EnemiesEvent);
        databases.Add(DungeonWave);
        databases.Add(Dungeon);

        databases.Add(Rarity);
        databases.Add(GearPerk);
        databases.Add(Equipment);
        databases.Add(Weapon);
        databases.Add(Buff);

        databases.Add(LevelExpAccount);
        databases.Add(LevelExpGameplay);
        databases.Add(LevelUpBuff);
        databases.Add(ShopWaveConfig);

    }

    public void LoadDatabase()
    {
        foreach (var database in databases)
        {
            database.Clear();
            database.GetDatabase();
        }
    }

    public void Reload()
    {
        databases.Clear();
        PreloadData();
        LoadDatabase();
    }
}