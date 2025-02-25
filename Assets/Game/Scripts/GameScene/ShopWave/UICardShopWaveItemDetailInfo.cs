using Assets.Game.Scripts.GameScene.ShopWave;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.Scripts.GameScene.Dungeon.ShopWave
{
    public class UICardShopWaveItemDetailInfo : MonoBehaviour
    {
        [SerializeField] private Text _debugNameText;
        [SerializeField] private Text m_NameCardTxt;
        [SerializeField] private Image m_IconCardImg;
        [SerializeField] private Image m_RarityIconCardImg;
        [SerializeField] private RectTransform m_StatDetailRect;

        private ShopWaveItemModel m_Model;
        public UniTask Setup(ShopWaveItemModel m_Model)
        {
            this.m_Model = m_Model;
            _debugNameText.text = m_Model.Id;
            return UniTask.CompletedTask;
        }
    }
}
