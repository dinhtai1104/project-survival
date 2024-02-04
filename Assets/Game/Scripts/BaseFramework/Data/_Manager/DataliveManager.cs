using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataliveManager : MonoSingleton<DataliveManager>
{
    public DungeonMapMenu DungeonLive;
    public void Init(Transform parent)
    {
        transform.SetParent(parent);
        DataManager.Live = this;
        DungeonLive = new DungeonMapMenu();
    }
}