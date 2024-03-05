using UnityEngine;
using System.Collections.Generic;
using System;
using Sirenix.OdinInspector;
using Assets.Game.Scripts.Core.Data.Database;
using Assets.Game.Scripts.Core.Data.Database.Dungeon;

public class DatabaseManager : MonoSingleton<DatabaseManager>
{
    private List<DataTable> databases = new List<DataTable>();

    public EnemyTable EnemyTable = new EnemyTable();
    public DungeonTable Dungeon = new DungeonTable();
    public DungeonRoomTable DungeonRoom = new DungeonRoomTable();
    public DungeonWaveTable DungeonWave = new DungeonWaveTable();

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
        databases.Add(DungeonRoom);
        databases.Add(DungeonWave);
        databases.Add(Dungeon);
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