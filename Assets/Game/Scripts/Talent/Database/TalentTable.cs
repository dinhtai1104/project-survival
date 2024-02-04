using Assets.Game.Scripts.BaseFramework.Architecture;
using BansheeGz.BGDatabase;
using GameUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Talent.Database
{
    [System.Serializable]
    public class TalentTable : DataTable<int, TalentEntity>
    {
        public override void GetDatabase()
        {
            DB_Talent.ForEachEntity(e => Get(e));
        }

        private void Get(BGEntity e)
        {
            var talent = new TalentEntity(e);
            if (!Dictionary.ContainsKey(talent.Id))
            {
                Dictionary.Add(talent.Id, talent);
            }
        }
        public TalentEntity GetTalentByIndex(int index)
        {
            return Dictionary[index];
        }
        public List<TalentEntity> GetTalentByQuality(ERarity quality)
        {
            return Dictionary.Values.ToList().FindAll(t => t.Quality == quality).ToList();
        }
        public List<TalentEntity> GetRandomTalent(int amount)
        {
            var listTalent = new List<TalentEntity>();
            var allTalent = Dictionary.Values.ToList();
            RemoveMaxLevelTalent(allTalent);
            for (int i = 0; i < amount; i++)
            {
                var talent = allTalent.RandomWeighted();
                listTalent.Add(allTalent[talent]);
                allTalent.RemoveAt(talent);
                if (allTalent.Count == 0) break;
            }
            return listTalent;
        }

        private void RemoveMaxLevelTalent(List<TalentEntity> allTalent)
        {
            TalentService service = Architecture.Get<TalentService>();
            for (int i = allTalent.Count - 1; i >= 0; i--)
            {
                if (service.IsMaxLevelTalent(allTalent[i].Id))
                {
                    allTalent.RemoveAt(i);
                }
            }
        }

        public List<TalentEntity> GetRollTalentSameWeight(int amount)
        {
            var listTalent = new List<TalentEntity>();
            var allTalent = Dictionary.Values.ToList();
            RemoveMaxLevelTalent(allTalent);
            for (int i = 0; i < amount; i++)
            {
                var talent = allTalent.Random();
                listTalent.Add(talent);
                allTalent.Remove(talent);
                if (allTalent.Count == 0) break;
            }
            return listTalent;
        }
    }
}
