using System;
using System.Collections.Generic;
using UnityEngine;

public class Boss30Skill3LazerPredict : ObjectSpawnAmountCircle<LineSimpleControl>
{
    public override List<LineSimpleControl> SpawnItem(int amount, float size, Action<LineSimpleControl> callback)
    {
        float angle = 0;
        float angleIncre = 360f / amount;
        for (int i = 0; i < amount; i++)
        {
            var lzer = PoolManager.Instance.Spawn(itemPrefabs, transform);
            lzer.transform.eulerAngles = new UnityEngine.Vector3(0, 0, angle);
            lzer.transform.localScale = UnityEngine.Vector3.one * size;
            angle += angleIncre;
            callback?.Invoke(lzer);
            items.Add(lzer);
        }
        return null;
    }
}