using UnityEngine;
using System.Collections.Generic;
using System;
using Sirenix.OdinInspector;
using Assets.Game.Scripts.Core.Data.Database;

public class DatabaseManager : MonoSingleton<DatabaseManager>
{
    private List<DataTable> databases = new List<DataTable>();

    public EnemyTable MonsterTable = new EnemyTable();
    public AdventureTable Adventure = new AdventureTable();

    public void Init(Transform parent = null)
    {
        DataManager.Base = this;
        if (parent) transform.SetParent(parent);

        PreloadData();
        LoadDatabase();
    }

    private void PreloadData()
    {
        databases.Add(MonsterTable);
        databases.Add(Adventure);
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