using Assets.Game.Scripts.Talent.Database;
using Assets.Game.Scripts.Talent.UI.Upgrade;
using Mosframe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.Talent.UI
{
    public class UITalentContainer : MonoBehaviour
    {
        public int ItemPerColumn = 3;

        private TalentTable table;
        [SerializeField] private DynamicHScrollView scrollView;
        private List<List<int>> IndexColumn = new List<List<int>>();
        public void Show()
        {
            table = DataManager.Base.Talent;
            int currentIndex = 0;
            int currentCol = 0;
            IndexColumn.Add(new List<int>());

            for (int i = 0; i < table.Dictionary.Count; i++)
            {
                if (currentIndex != 0 && currentIndex % ItemPerColumn == 0)
                {
                    IndexColumn.Add(new List<int>());
                    currentCol++;
                }
                IndexColumn[currentCol].Add(currentIndex);
                currentIndex++;
            }

            scrollView.init(IndexColumn.Count);
            scrollView.refresh();
        }

        public int GetIdTalent(int index)
        {
            var col = index / 3;
            var mod = col % 3;

            return IndexColumn[col][mod];
        }

        public bool IsValid(int index)
        {
            return index < table.Dictionary.Count;
        }

        public TalentEntity GetEntity(int id)
        {
            return table.GetTalentByIndex(id);
        }

        public void ScrollToIndex(int talentId)
        {
            var index = IndexColumn.FindIndex(t => t.Contains(talentId));
            scrollView.scrollFixByItemIndex(index);
        }

        public void EffectUpgrade(int talentId)
        {
            var index = IndexColumn.FindIndex(t => t.Contains(talentId));
            var item = scrollView.getItemIndex(index) as UITalentColumnRecycleItem;
            var itemTarget = item.GetItem(talentId);

            itemTarget.PunchScale();
        }
    }
}
