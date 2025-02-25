﻿using Assets.Game.Scripts.Events;
using Core;
using DG.Tweening;
using Engine;
using Pool;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.Manager
{
    public class LootObjectListener : MonoBehaviour
    {
        private void OnEnable()
        {
            GameArchitecture.GetService<IEventMgrService>().Subscribe<LastHitEventArgs>(LastHitEventHandler);
        }
        private void OnDisable()
        {
            GameArchitecture.GetService<IEventMgrService>().Unsubscribe<LastHitEventArgs>(LastHitEventHandler);
        }

        private void LastHitEventHandler(object sender, IEventArgs e)
        {
            var evt = e as LastHitEventArgs;
            if (evt.m_Patient is EnemyActor)
            {
                EnemyDiedProcess(evt.m_Patient as EnemyActor);
            }
            else if (evt.m_Patient is PlayerActor)
            {
                PlayerDiedProcess(evt.m_Patient as PlayerActor);
            }
        }

        private void EnemyDiedProcess(EnemyActor enemyActor)
        {
            if (MathUtils.RollChance(enemyActor.EntityData.PickleRatio))
            {
                var path = AddressableName.LootObject.AddParams("Pickle");
                var prefab = ResourcesLoader.Load<GameObject>(path);
                var position = enemyActor.CenterPosition;
                for (int i = 0; i < enemyActor.EntityData.PickleNumber; i++)
                {
                    var p = PoolFactory.Spawn(prefab, position, Quaternion.identity);
                    var rd = UnityEngine.Random.insideUnitCircle * 1.5f + (Vector2)position;
                    p.transform.DOMove(rd, 0.5f);
                }
            }
        }

        private void PlayerDiedProcess(PlayerActor playerActor)
        {
        }
    }
}