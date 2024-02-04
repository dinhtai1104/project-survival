using com.mec;
using Lean.Pool;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectSpawnAmount<TItem> : CharacterObjectBase where TItem : MonoBehaviour
{
    protected List<TItem> items = new List<TItem>();
    [SerializeField]
    protected TItem itemPrefabs;
    public abstract List<TItem> SpawnItem(int amount, float size, System.Action<TItem> callback);
    public virtual void Clear()
    {
        foreach (var item in items)
        {
            if (item != null)
            {
                if (item.gameObject.activeSelf)
                {
                    PoolManager.Instance.Despawn(item.gameObject);
                }
            }
        }
        items.Clear();
    }

    protected override IEnumerator<float> _OnUpdate()
    {
        while (true)
        {
            SkillEngine.Ticks();
            foreach (var i in listUpdateParallel)
            {
                i.OnUpdate();
            }

            if (items.Count == 0)
            {
                yield return Timing.WaitForSeconds(1f);
                if (items.Count == 0)
                {
                    break;
                }
            }
            yield return Timing.DeltaTime;
        }
        items.Clear();
        PoolManager.Instance.Despawn(gameObject);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        Clear();
    }
}