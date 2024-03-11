using Common;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Game.Scripts.GameScene.Dungeon.ShopWave;
using System;
using Cysharp.Threading.Tasks;

namespace Assets.Game.Scripts.GameScene.Dungeon.Main
{
    public class UIDungeonShopWavePanel : GamePanel
    {
        [SerializeField] private UICardShopWaveItem[] m_Items;

        public override void Show(params object[] @params)
        {
            base.Show(@params);

            ShowWait().Forget();
        }

        private async UniTaskVoid ShowWait()
        {
            await UniTask.Delay(1000);
            var listTask = new List<UniTask>();
            foreach (var item in m_Items)
            {
                var task = item.RotateCard(0.5f);
                listTask.Add(task);
                await UniTask.Delay(200);
            }

            await UniTask.WhenAll(listTask);
        }

        public void ContinueWaveOnClicked()
        {
            Close();
        }
    }
}
