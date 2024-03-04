using UnityEngine;
using System.Collections.Generic;
using System;
using Sirenix.OdinInspector;
using Assets.Game.Scripts.Core.Data.Database;
using Assets.Game.Scripts.Core.Data.Database.Dungeon;

public class DatabaseManager : MonoSingleton<DatabaseManager>
{
    private List<DataTable> databases = new List<DataTable>();

    public EnemyTable EntityTable = new EnemyTable();
    public DungeonTable DungeonTable = new DungeonTable();
    public DungeonRoomTable DungeonRoom = new DungeonRoomTable();

    public void Init(Transform parent = null)
    {
        DataManager.Base = this;
        if (parent) transform.SetParent(parent);

        PreloadData();
        LoadDatabase();
    }

    private void PreloadData()
    {
        databases.Add(EntityTable);
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