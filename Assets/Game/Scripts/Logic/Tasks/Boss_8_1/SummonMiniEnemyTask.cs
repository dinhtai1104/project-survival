using com.mec;
using Cysharp.Threading.Tasks;
using Game.GameActor;
using Sirenix.OdinInspector;
using Spine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.Logic.Tasks.Boss_8_1
{
    public class SummonMiniEnemyTask : AnimationRequireSkillTask
    {
        public bool RunOnStart = false;
        [ShowInInspector]
        public LayerMask ignoreMask => 256;
        public ValueConfigSearch enemySummons;
        public ValueConfigSearch numberEnemy;
        public ValueConfigSearch spaceFromCenter;
        public ValueConfigSearch distanceBtwEnemy;
        private List<Vector3> preparePosition;

        private string[] enemiesId;

        private void Awake()
        {
            enemiesId = enemySummons.StringValue.Split(';');
        }

        public override async UniTask Begin()
        {
            enemySummons.SetId(Caster.gameObject.name);
            numberEnemy.SetId(Caster.gameObject.name);
            spaceFromCenter.SetId(Caster.gameObject.name);
            distanceBtwEnemy.SetId(Caster.gameObject.name);

            preparePosition = GameUtility.GameUtility.GetRandomPositionWithoutOverlapping
                (Caster.GetMidPos(), spaceFromCenter.Vector2Value, distanceBtwEnemy.FloatValue, numberEnemy.IntValue, ignoreMask, true);
            await base.Begin();
            if (RunOnStart)
            {
                foreach (var pos in preparePosition)
                {
                    Spawn(pos, enemiesId.Random());
                }
                IsCompleted = true;
            }
        }

        protected override void AnimationHandler_onEventTracking(TrackEntry trackEntry, Spine.Event e)
        {
            base.AnimationHandler_onEventTracking(trackEntry, e);
            if (trackEntry.Animation.Name == animationSkill)
            {
                if (e.Data.Name == "attack_tracking")
                {
                    foreach (var pos in preparePosition)
                    {
                        Spawn(pos, enemiesId.Random());
                    }
                }
            }
        }

        protected async void Spawn(Vector3 pos, string enemyId)
        {
            var enemyhandler = GameController.Instance.GetEnemySpawnHandler();
            enemyhandler.SpawnSingle(enemyId, (int)Caster.GetStatValue(StatKey.Level), pos, usePortal: true, isStartBehaviourNow: false)
                .ContinueWith(enemyMini =>
                {
                    var enemyEntity = DataManager.Base.Enemy.Get(enemyId);
                    enemyMini.IsActived = false;
                    if (enemyEntity.MonsterTags.Contains(ETag.EnemyFly))
                    {
                        enemyMini.IsActived = true;
                        enemyMini.StartBehaviours();
                    }
                    else
                    {
                        Timing.RunCoroutine(_WaitForActive(4, enemyMini));
                        enemyMini.MoveHandler.onCharacterLanded += OnCharacterLand;
                    }
                }).Forget();
        }

        private IEnumerator<float> _WaitForActive(int v, Character enemyMini)
        {
            yield return Timing.WaitForSeconds(v);
            if (!enemyMini.IsActived)
            {
                enemyMini.IsActived = true;
                enemyMini.MoveHandler.onCharacterLanded -= OnCharacterLand;
                enemyMini.MoveHandler.Character.StartBehaviours();
            }
        }

        private void OnCharacterLand(MoveHandler moveHandler)
        {
            moveHandler.Character.IsActived = true;
            moveHandler.onCharacterLanded -= OnCharacterLand;
            moveHandler.Character.StartBehaviours();
        }
    }
}
