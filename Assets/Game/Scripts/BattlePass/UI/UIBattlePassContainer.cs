using ExtensionKit;
using Mosframe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.Scripts.BattlePass.UI
{
    public class UIBattlePassContainer : MonoBehaviour
    {
        public RectTransform bgProgressRect;
        public RectTransform progressRect;
        public DynamicHScrollView scroll;

        private BattlePassSaves save;
        private BattlePassService service;

        public void Show()
        {
            scroll.init(DataManager.Base.BattlePass.Dictionary.Count);
            scroll.refresh();
        }

        public void UpdateProgress()
        {
           // scroll.refresh();
        }

        public void SetProgress(int level)
        {
            progressRect.SetSizeWidth(level * scroll.itemPrototype.GetWidth());
        }
    }
}
