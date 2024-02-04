using com.assets.loader.addressables;
using com.assets.loader.core;
using Cysharp.Threading.Tasks;
using Game.GameActor;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.GameActor.Buff
{
    public class BuffHandler : MonoBehaviour, IBuffHandler
    {
        private ActorBase _caster;
        [ShowInInspector]
        private List<IBuff> buffs = new List<IBuff>();
        [ShowInInspector]
        private Dictionary<EBuff, IBuff> buffData = new Dictionary<EBuff, IBuff>();
        private IAssetLoader cardLoader = new AddressableAssetLoader();
        [SerializeField]
        private BuffsSave BuffSave = null;

        private void Awake()
        {
            Messenger.AddListener<ActorBase, EBuff>(EventKey.CastBuff, OnPickBuff);
            Messenger.AddListener<EBuff>(EventKey.CastBuffToMainPlayer, OnCastBuffToMain);
        }
        private void OnDestroy()
        {
            Messenger.RemoveListener<ActorBase, EBuff>(EventKey.CastBuff, OnPickBuff);
            Messenger.RemoveListener<EBuff>(EventKey.CastBuffToMainPlayer, OnCastBuffToMain);
        }

        private void OnCastBuffToMain(EBuff eBuff)
        {
            if (_caster == BattleGameController.Instance.GetMainActor())
            {
                Cast(eBuff);
            }
        }

        private void OnPickBuff(ActorBase caster, EBuff eBuff)
        {
            if (caster == _caster)
            {
                Cast(eBuff);
            }
        }

        public void Initialize(ActorBase caster)
        {
            _caster = caster;
            var session = GameController.Instance.GetSession();
            if (session == null) return;
            BuffSave = session.buffSession;
        }
        public async UniTask Cast(EBuff buff, bool save = true)
        {
            var buffIns = await ApplyBuff(buff, save);
            buffIns.BeforePlay();
            buffIns.Play();
        }

        private async UniTask<IBuff> ApplyBuff(EBuff buff, bool save = true)
        {
            if (save)
            {
                // Save
                BuffSave.Dungeon.Buff(buff, BuffSave.Dungeon.StageId);
            }
            if (buffData.ContainsKey(buff))
            {
                buffData[buff].BuffData.LevelUp();
            } 
            else
            {
                var entity = DataManager.Base.Buff.Dictionary[buff];
                var ability = await LoadBuff(buff);
                ability.Initialize(_caster, entity, BuffSave.Dungeon.StageId);
                buffs.Add(ability);
                buffData.Add(buff, ability);
            }
            return buffData[buff];
        }

        private async UniTask<IBuff> LoadBuff(EBuff buff)
        {
            var prePath = "Buff";
            if (buff >= EBuff.NormalHeroPassive)
            {
                prePath = "Hero";
            }
            if (buff >= EBuff.TempEquipmentPassive)
            {
                prePath = "Equipment";
            }
            var path = $"{prePath}/{buff}.prefab";
            var op = cardLoader.LoadAsync<GameObject>(path);
            await op.Task;

            var buffIns = PoolManager.Instance.Spawn(op.Result, transform);
            cardLoader.Release(op);
            return buffIns.GetComponent<IBuff>();
        }

        public IBuff GetBuff(EBuff buff)
        {
            return null;
        }

        public void Ticks()
        {
            for (int i = 0; i < buffs.Count; i++)
            {
                if (buffs[i] != null)
                {
                    buffs[i].OnUpdate(Time.deltaTime);
                }
            }
        }

        [Button]
        public void SubstractHeal()
        {
            _caster.HealthHandler.AddHealth(-20);
            Debug.Log("Heal Current: " + _caster.HealthHandler.GetHealth());
        }

        public void Remove(EBuff buff)
        {
            var buffIns = buffData[buff];
            buffData.Remove(buff);
            buffs.RemoveAll(t => t.BuffData.Type == buff);
            BuffSave.RemoveBuff(buff);
            buffIns.Exit();
        }

        public async UniTask CastOnSave(EBuff buff, bool use)
        {
            var buffIns = await ApplyBuff(buff, false);
            if (use)
            {
                buffIns.BeforePlay();
                buffIns.Play();
            }
        }

        public bool HasBuff(EBuff buff)
        {
            return buffData.ContainsKey(buff);
        }

        public async UniTask Cast(IBuff buff)
        {
            var entity = DataManager.Base.Buff.Dictionary[buff.BuffKey];
            buff.Initialize(_caster, entity, 1);
            buffs.Add(buff);
            buffData.Add(buff.BuffKey, buff);

            buff.BeforePlay();
            buff.Play();
            await UniTask.Yield();
        }
    }
}