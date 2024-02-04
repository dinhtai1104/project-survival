using Cysharp.Threading.Tasks;
using Effect;
using Game;
using Game.Pool;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UI;
using UnityEngine;

namespace Game.GameActor
{
    public class PlayerController : Character, IBatchUpdate
    {
        public EHero Hero;
        [SerializeField]
        MoreMountains.Feedbacks.MMF_Player getHitFeedBack,deadFeedback;
        public override ECharacterType GetCharacterType()
        {
            return ECharacterType.Player;
        }
        protected override async UniTask SetUp()
        {
            await base.SetUp();
            Tagger.AddTag(ETag.Player);
            AddBuffHero();
            onRevive += GameController.Instance.OnMainPlayerReviveResult;
        }
        float effectTime = 0;
        public override bool GetHit(DamageSource damageSource, IDamageDealer dealer)
        {
            if( base.GetHit(damageSource, dealer))
            {
                //handle effect when player get hit
                if (Time.time - effectTime > 0.25f)
                {
                    if (soundData != null && UnityEngine.Random.Range(0f, 1f) > 0.9f)
                        this.SoundHandler.PlayOneShot(soundData.hurtSFXs[UnityEngine.Random.Range(0, soundData.hurtSFXs.Length)], 1f);

                  

                }
                getHitFeedBack?.PlayFeedbacks();
                return true;
            }
            return false;
        }
        public override void Dead()
        {
            base.Dead();
            deadFeedback?.PlayFeedbacks();
        }

        public override void OnDead()
        {
            //WeaponHandler.Destroy();
            BehaviourHandler.Destroy();

            SetActive(false);
            OnReviveEvent();
        }
        protected override async void OnReviveEvent()
        {
            var reviveTimes = Stats.GetValue(StatKey.Revive);
            if (reviveTimes > 0)
            {
                onRevive?.Invoke(true);
                await Revive(transform.position);
                return;
            }
            else
            {
                if (!GameController.Instance.GetSession().IsRevived)
                {
                    // Show popup
                    if (GameController.Instance.GetSession().CurrentStage * 1.0f / GameController.Instance.GetDungeonEntity().Stages.Count >= new ValueConfigSearch("[Revive]AllowStages", "10").IntValue / 100f)
                    {
                        var ui = await PanelManager.CreateAsync<UIRevivePanel>(AddressableName.UIRevivePanel);
                        ui.Show((value) =>
                        {
                            onRevive?.Invoke(value);
                            if (value)
                            {
                                Revive(transform.position).Forget();
                                GameController.Instance.GetSession().IsRevived = true;
                                GameController.Instance.GetSession().Save();
                            }
                        });
                        return;
                    }
                }
            }
            onRevive?.Invoke(false);
        }

        private void AddBuffHero()
        {
            //var data = GameSceneManager.Instance.PlayerData;
            var heroId = (int)Hero;
            var heroBuffPassive = (EBuff)(1000 + heroId);
            if (BuffHandler.HasBuff(heroBuffPassive) == false)
            {
                BuffHandler.Cast(heroBuffPassive, false);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            UpdateSlicer.Instance.RegisterSlicedUpdate(this, UpdateSlicer.UpdateMode.Always );
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            onRevive -= GameController.Instance.OnMainPlayerReviveResult;
        }
        protected override void OnDisable()
        {
            base.OnDisable();

            UpdateSlicer.Instance.DeregisterSlicedUpdate(this);
        }
        public override void OnVisible(bool isVisible)
        {
          
        }
        //private Vector3 aimDirection;
        public void BatchUpdate()
        {
            //if (IsDead() || IsStun()) return;

          
            //if (weaponInstance != null && GetWeaponHandler() != null)
            //{
            //    if (manuallyShootDirection.SqrMagnitude() == 0)
            //    {
            //        GetWeaponHandler().Rotate(this.MoveHandler.lastMove);
            //        GetWeaponHandler().Flip(this.MoveHandler.lastMove);
            //        onAimAtTarget?.Invoke(this, this.MoveHandler.lastMove, null);
            //        aimDirection = this.MoveHandler.lastMove.normalized;
            //        if (aimDirection.x >= 0 && aimDirection.x < 0.5f) aimDirection.x = 0.5f;
            //        if (aimDirection.x < 0 && aimDirection.x > -0.5f) aimDirection.x = -0.5f;
                   
            //    }
            //    else
            //    {
            //        GetWeaponHandler().Rotate(manuallyShootDirection);
            //        GetWeaponHandler().Flip(manuallyShootDirection);

            //        // prevent facing too down below or above
            //        aimDirection = manuallyShootDirection.normalized ;
            //        if (aimDirection.x >= 0 && aimDirection.x < 0.8f) aimDirection.x = 0.8f;
            //        if (aimDirection.x < 0 && aimDirection.x > -0.8f) aimDirection.x = -0.8f;


            //        ITarget target = FindClosetTarget();
            //        onAimAtTarget?.Invoke(this, manuallyShootDirection, target);
            //    }
            //    // player look at target
            //    aimTransform.position = lookTransform.position + aimDirection;
            //}

        }

        public void BatchFixedUpdate()
        {
        }



    }
}