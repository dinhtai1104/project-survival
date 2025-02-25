using Assets.Game.Scripts.Enums;
using Assets.Game.Scripts.Objects.Loots;
using Engine;
using ExtensionKit;
using Pool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.Manager
{
    [System.Serializable]
    public class LootObjectCacheModel
    {
        [SerializeField] private Vector3 _position;
        [SerializeField] private ELootObject _lootType;

        public Vector3 Position
        {
            get => _position;
            set => _position = value;
        }

        public ELootObject LootType
        {
            get => _lootType;
            set => _lootType = value;
        }
    }

    [System.Serializable]
    public class LootObjectUpdater : ClassSingleton<LootObjectUpdater>
    {
        private Camera m_Camera;
        private HashSet<int> m_Lookup = new HashSet<int>();
        private List<LootableObject> m_LootObjects = new List<LootableObject>();
        private List<LootObjectCacheModel> m_LootCacheObjects = new List<LootObjectCacheModel>();
        private float m_Time = 0;
        public void OnUpdate()
        {
            for (int i = m_LootObjects.Count - 1; i >= 0; i--)
            {
                var loot = m_LootObjects[i];
                if (loot != null)
                {
                    if (!CheckVisiblity(loot.transform.position))
                    {
                        // Create cache then store them
                        var model = new LootObjectCacheModel
                        {
                            Position = loot.transform.position,
                            LootType = loot.Type
                        };

                        m_LootCacheObjects.Add(model);
                        loot.Destroy();
                    }
                    else
                    {
                        loot.OnUpdate();
                    }
                }
                else
                {
                    
                }
            }

            m_Time += Time.deltaTime;
            if (m_Time >= 0.1f)
            {
                // Try Recreate loot object
                TryRecreateLootObject();
                m_Time = 0f;
            }
        }

        private void TryRecreateLootObject()
        {
            for (int i = m_LootCacheObjects.Count - 1; i >= 0; i--)
            {
                var cache = m_LootCacheObjects[i];
                if (CheckVisiblity(cache.Position))
                {
                    // Create again
                    var path = AddressableName.LootObject.AddParams(cache.LootType);
                    var prefab = ResourcesLoader.Load<GameObject>(path);
                    if (prefab != null)
                    {
                        var obj = PoolFactory.Spawn(prefab, cache.Position, Quaternion.identity)
                            .GetComponent<LootableObject>();
                    }
                    m_LootCacheObjects.RemoveAt(i);
                }
            }
        }

        public void RemoveObject(LootableObject obj)
        {
            var instanceId = obj.GetInstanceID();
            if (m_Lookup.Contains(instanceId))
            {
                m_Lookup.Remove(instanceId);
                m_LootObjects.Remove(obj);
            }
        }
        public void AddObject(LootableObject obj)
        {
            var instanceId = obj.GetInstanceID();
            if (!m_Lookup.Contains(instanceId))
            {
                m_Lookup.Add(instanceId);
                m_LootObjects.Add(obj);
            }
        }

        public bool CheckVisiblity(Vector3 position)
        {
            var viewPos = m_Camera.WorldToViewportPoint(position);
            var onScreen = viewPos.x > 0f && viewPos.x < 1f && viewPos.y > 0f && viewPos.y < 1f;
            return onScreen;
        }

        public LootableObject GetNearest(Vector3 centerPosition, float rangeLookup)
        {
            LootableObject target = null;
            float minDis = 1000;
            for (int i = 0; i < m_LootObjects.Count; i++)
            {
                var pos = m_LootObjects[i].GetPosition();
                var range = (pos - centerPosition).magnitude;
                if (range < minDis)
                {
                    minDis = range;
                    target = m_LootObjects[i];
                }
            }
            if (minDis < rangeLookup)
            {
                return target;
            }
            return null;
        }

        public void DestroyAll()
        {
            for (int i = m_LootObjects.Count - 1; i >= 0; i--)
            {
                var loot = m_LootObjects[i];
                RemoveObject(m_LootObjects[i]);
                loot.Destroy();
            }
            m_LootCacheObjects.Clear();
        }

        public LootObjectUpdater() : base()
        {
            m_Camera = Camera.main;
        }
    }
}
