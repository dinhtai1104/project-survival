using Cysharp.Threading.Tasks;
using Game.GameActor;
using Game.Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.Actor
{
    public class CannotTargetThisActor : MonoBehaviour
    {
        public Transform hitBox;
        private ActorBase actorBase;
        private void Awake()
        {
            actorBase = GetComponent<ActorBase>();
            hitBox = transform.Find("Hitbox");

            if (hitBox != null)
            {
                hitBox.gameObject.SetActive(false);
            }
        }
        private async void OnEnable()
        {
            await UniTask.Delay(500);
            actorBase.Tagger.AddTag(ETag.Immune);
        }
    }
}
