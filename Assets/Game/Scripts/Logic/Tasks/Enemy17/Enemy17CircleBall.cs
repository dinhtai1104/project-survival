using System;
using System.Collections.Generic;

public class Enemy17CircleBall : ObjectSpawnAmountCircle<CharacterObjectBase>
{
    public override List<CharacterObjectBase> SpawnItem(int amount, float size, Action<CharacterObjectBase> callback)
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