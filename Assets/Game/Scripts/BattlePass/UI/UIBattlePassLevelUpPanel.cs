using Game.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Playables;

namespace Assets.Game.Scripts.BattlePass.UI
{
    public class UIBattlePassLevelUpPanel : Panel
    {
        [SerializeField] private TextMeshProUGUI levelTxt;

        public TaskRunner taskClose;
        private bool isClose = false;
        public override void PostInit()
        {
            isClose = false;
        }
        public void Show(int From, int To)
        {
            base.Show();
            levelTxt.text = To.ToString();
        }

        public override void Close()
        {
            if (isClose) return;
            taskClose.RunTask();
            taskClose.OnComplete += () =>
            {
                base.Close();
            };
        }
    }
}
