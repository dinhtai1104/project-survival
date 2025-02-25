using Assets.Game.Scripts.DataGame.Loot;
using Assets.Game.Scripts.Enums;
using Assets.Game.Scripts.Manager;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityRenderer;
using DG.Tweening;
using Pool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Game.Scripts.Objects.Loots
{
    public abstract class LootableObject : MonoBehaviour
    {
        [SerializeField] protected bool m_IsInvisible;
        [SerializeField] protected ELootObject m_LootType;

        private bool m_IsLooted = false;

        public bool IsLooted => m_IsLooted;
        public bool IsInvisible => m_IsInvisible;
        public ELootObject Type => m_LootType;
        public virtual void Loot()
        {
            LootObjectUpdater.Instance.RemoveObject(this);
        }
        public virtual void OnUpdate() { }
        protected virtual void OnEnable()
        {
            LootObjectUpdater.Instance.AddObject(this);
            // Jump
        }
        protected virtual void OnDisable() 
        {
            m_IsInvisible = false;
            LootObjectUpdater.Instance.RemoveObject(this);
        }

        public void Destroy()
        {
            m_IsInvisible = true;
            PoolFactory.Despawn(gameObject);
        }
    }
}
