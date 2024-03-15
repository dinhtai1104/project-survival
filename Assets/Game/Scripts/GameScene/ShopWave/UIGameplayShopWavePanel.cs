using Assets.Game.Scripts.GameScene.Dungeon.ShopWave;
using Common;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.GameScene.ShopWave
{
    public class UIGameplayShopWavePanel : GamePanel
    {
        [SerializeField] private UICardShopWaveItem[] m_Items;
        private ShopWaveHandler m_ShopWaveHandler;
        private int m_CurrentRolled = 0;
        public override void Show(params object[] @params)
        {
            base.Show(@params);
            if (@params.Length > 0 )
            {
                m_ShopWaveHandler = @params[0] as ShopWaveHandler;
            }
            Request(false);
        }

        private void Request(bool reroll)
        {
            if (reroll)
            {
                m_CurrentRolled++;
            }
            var rs = m_ShopWaveHandler.RequestShopWave(3);

            ShowWait(rs).Forget();
        }

        private async UniTaskVoid ShowWait(List<ShopWaveItemModel> models)
        {
            var listTask = new List<UniTask>();

            for (int i = 0; i < models.Count; i++)
            {
                m_Items[i].Setup(models[i]);
            }

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
