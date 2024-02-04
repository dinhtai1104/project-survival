using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DungeonSaveSO : SOBaseGenerics<DungeonBuffData>
{
    public void Apply(DungeonBuffData data)
    {
        Data.StageId = data.StageId;
        Data.DungeonId = data.DungeonId;
        Data.BuffEquiped.Clear();
        foreach (var i in data.BuffEquiped)
        {
            Data.BuffEquiped.Add(i.Key, i.Value);
        }
        Data.Save = data.Save;
    }

    public Dictionary<EBuff, BuffSave> GetData() => Data.BuffEquiped;
}