using GoogleMobileAds.Api;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class ObjectSpawnAmountCircle<T> : ObjectSpawnAmount<T> where T : MonoBehaviour
{
    protected float radius;
    protected float size;
    protected float angleStart;
    protected Vector3 position;
    public void SetRadius(float radius) { this.radius = radius; }
    public void SetAngleStart(float angleStart) { this.angleStart = angleStart; }
    public void SetPosition(Vector3 position) { this.position = position; }
    public override List<T> SpawnItem(int amount, float size, System.Action<T> callback)
    {
        this.size = size;
        GenObjects(amount);

        foreach (var item in items)
        {
            callback?.Invoke(item);
        }
        return items;
    }

    public override void Clear()
    {
    }

    private void GenObjects(int number)
    {
        items.Clear();
        float incre = 360f / number;
        float angleNow = angleStart;

        for (int i = 0; i < number; i++)
        {
            var bl = PoolManager.Instance.Spawn(itemPrefabs, transform.parent);
            bl.transform.localScale = size * Vector3.one;
            bl.transform.localEulerAngles = new Vector3(0, 0, angleNow);
            bl.transform.position = position + bl.transform.right * radius;

            angleNow += incre;
            items.Add(bl);
        }
    }
}