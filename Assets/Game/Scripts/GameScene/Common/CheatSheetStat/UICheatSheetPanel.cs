using Common;
using Cysharp.Threading.Tasks;
using Gameplay;
using Manager;
using Pool;
using System;
using System.Collections.Generic;
using Ui.View;
using UnityEngine;

namespace Assets.Game.Scripts.GameScene.Common.CheatSheetStat
{
    public class UICheatSheetPanel : GamePanel
    {
        [SerializeField] private UIStatItem m_StatItemPrefab;
        [SerializeField] private RectTransform m_ContentStatHolder;

        private List<UIStatItem> m_StatItemGO = new List<UIStatItem>();
        public override void Show(params object[] @params)
        {
            base.Show(@params);
            var mainPlayer = (SceneController as BaseGameplayScene).MainPlayer.Stats;

            foreach (var statKey in mainPlayer.StatNames)
            {
                var item = PoolFactory.Spawn(m_StatItemPrefab, m_ContentStatHolder);
                item.Setup(statKey, mainPlayer.GetStat(statKey));
                m_StatItemGO.Add(item);
            }
            onClosed += OnClosed;
        }

        private void OnClosed()
        {
            foreach (var stat in m_StatItemGO)
            {
                PoolFactory.Despawn(stat.gameObject);
            }
            m_StatItemGO.Clear();
        }
    }
}
