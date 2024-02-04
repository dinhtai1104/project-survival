using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Pool
{
    public class UIPoolHandler:MonoBehaviour
    {
        public List<UIPoolComponent> pool = new List<UIPoolComponent>();
        private List<UIPoolComponent> busyPool = new List<UIPoolComponent>();
        
        [SerializeField]
        private GameObject[] prefab;
        Transform holder;
        int index = 0;
        public UIPoolComponent Get(int prefabIndex)
        {
            if (holder == null)
            {
                this.holder = transform;
                if(prefab==null || prefab.Length == 0)
                {
                    prefab = new GameObject[transform.childCount];
                    int index = 0;
                    foreach(Transform child in transform)
                    {
                        prefab[index++] = child.gameObject;
                    }
                }
            }
            if (pool.Count > 0)
            {
                UIPoolComponent result = pool[pool.Count - 1];

                pool.Remove(result);
                busyPool.Add(result);

                return result;
            }
            GameObject obj = GameObject.Instantiate(prefab[prefabIndex], holder) as GameObject;
            UIPoolComponent t = obj.AddComponent<UIPoolComponent>();
            t.gameObject.name += index++;
            busyPool.Add(t);
            t.OnInitialized(this);

            return t;
        }
        public void Release(UIPoolComponent poolComponent)
        {
            if (busyPool.Contains(poolComponent))
            {
                busyPool.Remove(poolComponent);
                pool.Add(poolComponent);
            }
        }
        public void Clear()
        {
            busyPool.Reverse();
            pool.AddRange(busyPool);
            busyPool.Clear();
        }
    }
}