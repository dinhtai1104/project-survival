using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GameActor
{
    public class AttackHandler : MonoBehaviour
    {
        public delegate void OnShoot(Character character);
        public OnShoot onShoot;
        public delegate void OnStop();
        public OnStop onStop;
        protected Character character;
        public bool active = false, isBusy = false, canTrigger = false,triggerAllWeaponAtOnce=false;
        public LayerMask targetMask;
        public ECharacterType targetType;
        public float lastSuccessAttack;


        public void SetUp(Character character)
        {
            this.character = character;
            active = true;
        }
        public void Active()
        {
            canTrigger = true;
        }

        public void Stop()
        {
            canTrigger = false;

            if (character != null && character.WeaponHandler.CurrentWeapon != null)
                character.WeaponHandler.CurrentWeapon.Release();

            onStop?.Invoke();
        }
        public virtual void Trigger()
        {
            WeaponBase currentWeapon = character.WeaponHandler.CurrentWeapon;
            if (character.Sensor.CurrentTarget == null) return;
            if (active && !character.IsStun() && currentWeapon != null && currentWeapon.Trigger(character.WeaponHandler.GetAttackPoint(currentWeapon),
                target:character.Sensor.CurrentTarget.GetMidTransform(),
                facing: character.GetLookDirection(),
                trackingTarget: character.Sensor.CurrentTarget,
                OnAttackEnded))
            {
                isBusy = true;
                onShoot?.Invoke(character);
                character.AnimationHandler.SetShoot();
                lastSuccessAttack = Time.time;

            }
        }
      
        bool IsBlocked(Vector3 pos,Vector2 direction)
        {
            return Physics2D.Raycast(pos, direction, 0.1f, layerMask: character.Sensor.wallMask).collider != null;
        }
        public virtual void Trigger(Vector2 direction,ITarget trackingTarget)
        {
            WeaponBase currentWeapon = character.WeaponHandler.CurrentWeapon;

            Transform triggerPos = character.WeaponHandler.GetAttackPoint(currentWeapon);
            if (active && trackingTarget != null && !character.IsStun() && currentWeapon != null && currentWeapon.CanTrigger() &&!IsBlocked(triggerPos.position, direction) && currentWeapon.Trigger(triggerPos,
                 target: trackingTarget.GetMidTransform(),
                facing: direction,
                trackingTarget: trackingTarget
                , OnAttackEnded))
            {
                isBusy = true;
                onShoot?.Invoke(character);
                character.AnimationHandler?.SetShoot();
                lastSuccessAttack = Time.time;

            }
        }
        public virtual void TriggerSupportWeapons()
        {
            int index = 1;
            bool isTriggered = false;
            foreach(var supportWeapon in character.WeaponHandler.SupportWeapons)
            {
                ITarget trackingTarget = character.Sensor.GetTarget(index);
                Vector3 direction = (trackingTarget.GetMidTransform().position - character.WeaponHandler.GetAttackPoint(supportWeapon).position).normalized;
                if (active && trackingTarget != null && !character.IsStun() && supportWeapon != null && supportWeapon.Trigger(character.WeaponHandler.GetAttackPoint(supportWeapon),
                     target: trackingTarget.GetMidTransform(),
                    facing: direction,
                    trackingTarget: trackingTarget
                    , OnAttackEnded))
                {
                    isTriggered = true;

                    index++;
                }
            }
            if (isTriggered)
            {
                isBusy = true;
                onShoot?.Invoke(character);
                character.AnimationHandler?.SetShoot();
                lastSuccessAttack = Time.time;
            }
         
        }

        private void Update()
        {
            if (canTrigger)
            {
                ITarget target = character.Sensor.CurrentTarget;
                if (target != null)
                {
                    Trigger(character.GetLookDirection(), target);

                    TriggerSupportWeapons();
                }
               
            }
        }
        public void OnAttackEnded()
        {
            Messenger.Broadcast(EventKey.OnAttack, character);
            isBusy = false;
            lastSuccessAttack = Time.time;
        }

        public bool IsValid(ECharacterType type)
        {
            return targetType.Contains(type);
        }
    }
}
