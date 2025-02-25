using Assets.Game.Scripts.GameScene.ShopWave;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using ExtensionKit;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.GameScene.Dungeon.ShopWave
{
    public class UICardShopWaveItem : MonoBehaviour
    {
        [SerializeField] private GameObject m_PresentObj;
        [SerializeField] private UICardShopWaveItemDetailInfo m_CardInfoObj;
        [SerializeField] private AnimationCurve m_RotateFirstCurve;
        [SerializeField] private AnimationCurve m_RotateSecondCurve;

        private ShopWaveItemModel m_Model;

        public void Setup(ShopWaveItemModel model)
        {
            this.m_Model = model;
            m_PresentObj.SetActive(true);
            m_CardInfoObj.SetGOActive(false);

            SetupInformation().Forget();
        }

        private async UniTaskVoid SetupInformation()
        {
            await m_CardInfoObj.Setup(m_Model);
            await UniTask.Yield();
        }

        [Button]
        public async UniTask RotateCard(float duration)
        {
            float step = duration / 2;
            transform.DOScale(Vector3.one * 1.1f, step).SetEase(m_RotateFirstCurve);
            await Rotate(0, 90, m_RotateFirstCurve);
            m_PresentObj.SetActive(false);
            m_CardInfoObj.SetGOActive(true);

            transform.DOScale(Vector3.one, step).SetEase(m_RotateSecondCurve);
            await Rotate(90, 0, m_RotateSecondCurve);


            async UniTask Rotate(float from, float target, AnimationCurve curve) 
            {
                float timeCurrent = 0;
                float angleCurrent = 0;

                while (timeCurrent < step)
                {
                    angleCurrent = Mathf.Lerp(from, target, curve.Evaluate(timeCurrent / step));
                    transform.rotation = Quaternion.Euler(0, angleCurrent, 0);
                    timeCurrent += Time.deltaTime;
                    await UniTask.Yield();
                }
            }
        }
    }
}
