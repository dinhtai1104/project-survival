using Assets.Game.Scripts.Core.Data.Database.Buff;
using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.DataGame.Data
{
    [System.Serializable]
    public class BuffData
    {
        public int IdBuff;
        public string Type;
        public int LevelBuff;

        [NonSerialized]
        public BuffEntity BuffEntity;
        public bool IsUsePrefab => BuffEntity.IsPrefabBuff;

        public void Buff()
        {
            LevelBuff++;
        }
        
        public void Debuff()
        {
            LevelBuff--;
        }

        public void ApplyBuff(IStatGroup stat)
        {
            var db = DataManager.Base.Buff.Get(IdBuff).Clone();

            foreach (var passive in db.ModifierPassive)
            {
                if (!stat.HasStat(passive.AttributeName))
                {
                    stat.AddStat(passive.AttributeName, 0);
                }

                stat.AddModifier(passive.AttributeName, passive.Modifier, this);
                Debug.Log(passive.ToString() + " ::: Value: " + stat.GetValue(passive.AttributeName));
            }
        }

        public float GetValue(string statKey, EModifierBuff mod)
        {
            switch (mod)
            {
                case EModifierBuff.Passive:
                    foreach (var ev in BuffEntity.ModifierPassive)
                    {
                        if (ev.AttributeName == statKey)
                        {
                            return ev.Modifier.Value;
                        }
                    }
                    break;
                case EModifierBuff.Skill:
                    foreach (var ev in BuffEntity.ModifierSkill)
                    {
                        if (ev.AttributeName == statKey)
                        {
                            return ev.Modifier.Value;
                        }
                    }
                    break;
            }
            return 0;
        }
    }

    public enum EModifierBuff
    {
        Passive,
        Skill,
    }
}
