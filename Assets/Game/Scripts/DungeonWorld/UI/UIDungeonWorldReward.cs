using Assets.Game.Scripts.DungeonWorld.Data;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UIDungeonWorldReward : MonoBehaviour
{
    [SerializeField] private List<UIDungeonWorldStageItem> stagesItem;

    [HideInInspector]
    public UIDungeonWorldItem item;
    private DungeonWorldEntity entity;
    public void Set(DungeonWorldEntity entity)
    {
        this.entity = entity;
        int index = 0;
        foreach (var item in stagesItem)
        {
            item.Set(entity.Stages[index++]);
        }
    }

    [Button]
    public void Test(int dungeon)
    {
        var ent = DataManager.Base.DungeonWorld.Get(dungeon);
        Set(ent);
    }

    public void DeRegister()
    {
        item.Setup();
    }
}