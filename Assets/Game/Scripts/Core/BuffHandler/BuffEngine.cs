using Assets.Game.Scripts.DataGame.Data;
using Engine;
using Pool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.Core.BuffHandler
{
    public class BuffEngine : MonoBehaviour, IBuffEngine
    {
        private Actor m_Owner;
        private List<BuffData> m_BuffData = new List<BuffData>();
        private List<BuffGameObject> m_BuffInstance = new List<BuffGameObject>();
        public Actor Owner => m_Owner;

        public void Init(Actor owner)
        {
            m_Owner = owner;
        }
        public void AddBuff(BuffData buff)
        {
            var buffLocal = GetBuff(buff.IdBuff);
            if (buffLocal == null)
            {
                m_BuffData.Add(buff);
                buff.ApplyBuff(Owner.Stats);

                if (buff.IsUsePrefab)
                {
                    // Spawn Prefab Buff
                    var path = buff.BuffEntity.PrefabPath;
                    var buffPrefab = ResourcesLoader.Load<GameObject>(path);
                    if (buffPrefab != null)
                    {
                        var buffInstance = PoolFactory.Spawn(buffPrefab).GetComponent<BuffGameObject>();
                        buffInstance.Init(Owner, buff);
                    }
                    else
                    {
                        Debug.LogError("Buff " + path + " Not Exist in Project");
                    }
                }
            }
            else
            {
                buffLocal.Buff();
                buffLocal.ApplyBuff(Owner.Stats);
            }

            Debug.Log("Add success buff: " + buff.Type + " into Character " + Owner.name); 
        }

        public void Debuff(BuffData buff)
        {
            var buffLocal = GetBuff(buff.IdBuff);
            if (buffLocal == null)
            {
                Debug.LogError("Not Found Buff In Character: " + Owner.name);
                return;
            }
            buffLocal.Debuff();
            m_BuffData.Remove(buffLocal);
        }

        public BuffData GetBuff(int Id)
        {
            foreach (var buff in m_BuffData)
            {
                if (buff.IdBuff == Id) return buff;
            }
            return null;
        }

        public BuffData GetBuff(string Type)
        {
            foreach (var buff in m_BuffData)
            {
                if (buff.Type == Type) return buff;
            }
            return null;
        }

        public void OnUpdate()
        {
            for (int i = m_BuffInstance.Count - 1; i >= 0; i--)
            {
                var buff = m_BuffInstance[i];
                if (buff != null)
                {
                    buff.OnUpdate();
                }
            }

#if DEVELOPMENT
            if (Input.GetKeyDown(KeyCode.B))
            {
                PickRandomBuff();
            }
#endif
        }

#if DEVELOPMENT
        private void PickRandomBuff()
        {
            var random = DataManager.Base.Buff.GetRandom();
            var buffData = new BuffData
            {
                IdBuff = random.Id,
                Type = random.Type,
                LevelBuff = 0,
                BuffEntity = random
            };
            AddBuff(buffData);
        }
#endif
    }
}
