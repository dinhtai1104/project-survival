using Mosframe;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Game.Scripts.Talent.UI
{
    public class UITalentColumnRecycleItem : UIBehaviour, IDynamicScrollViewItem
    {
        public UITalentContainer _container;
        private int index = -1;
        public UITalentItem talentItemPrefab;
        public List<RectTransform> holderItem;
        private List<UITalentItem> items = new List<UITalentItem>();
        public int getIndex()
        {
            return index;
        }

        public void onUpdateItem(int index)
        {
            this.index = index;
            UpdateUI();
        }

        private void UpdateUI()
        {
            Clear();
            int startIndex = index * _container.ItemPerColumn;

            for (int i = startIndex; i < startIndex + 3; i++)
            {
                if (!_container.IsValid(i))
                {
                    break;
                }
                var ui = PoolManager.Instance.Spawn(talentItemPrefab, holderItem[i - startIndex]);
                ui.Setup(_container.GetEntity(i));
                ui.transform.localPosition = Vector3.zero;
                items.Add(ui);
            }
        }

        private void Clear()
        {
            foreach (var go in items)
            {
                PoolManager.Instance.Despawn(go.gameObject);
            }
            items.Clear();
        }

        public UITalentItem GetItem(int talentId)
        {
            return items.Find(t => t.Entity.Id == talentId);
        }
    }
}
