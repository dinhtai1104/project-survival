using UnityEngine;
using System.Collections.Generic;
using System;
using Sirenix.OdinInspector;
using Assets.Game.Scripts.Core.Data.Database;
using Assets.Game.Scripts.Core.Data.Database.Dungeon;
using Assets.Game.Scripts.Core.Data.Database.Equipment.Gear;
using Assets.Game.Scripts.Core.Data.Database.Equipment;
using Assets.Game.Scripts.Core.Data.Database.Equipment.Weapon;
using Assets.Game.Scripts.Core.Data.Database.Rarity;
using Assets.Game.Scripts.Core.Data.Database.Dungeon.EnemyEvent;

public class DatabaseManager : MonoSingleton<DatabaseManager>
{
    private List<DataTable> databases = new List<DataTable>();

    public EnemyTable EnemyTable = new EnemyTable();
    public DungeonTable Dungeon = new DungeonTable();
    public EnemiesEventTable EnemiesEvent = new EnemiesEventTable();
    public DungeonWaveTable DungeonWave = new DungeonWaveTable();

    public RarityTable Rarity = new RarityTable();
    public GearTable RarityGear = new GearTable();
    public EquipmentTable Equipment = new EquipmentTable();
    public WeaponTable Weapon = new WeaponTable();

    public void Init(Transform parent = null)
    {
        DataManager.Base = this;
        if (parent) transform.SetParent(parent);

        PreloadData();
        LoadDatabase();
    }

    private void PreloadData()
    {
        databases.Add(EnemyTable);
        databases.Add(EnemiesEvent);
        databases.Add(DungeonWave);
        databases.Add(Dungeon);

        databases.Add(Rarity);
        databases.Add(RarityGear);
        databases.Add(Equipment);
        databases.Add(Weapon);
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