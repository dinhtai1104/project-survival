using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Engine
{
    public class NullGraphicEngine : IGraphicEngine
    {
        public ActorBase Owner { get; private set; }

        public void Init(ActorBase actor)
        {
            Owner = actor;
        }

        public void SetActiveRenderer(bool active)
        {
        }

        public void SetGraphicAlpha(float a)
        {
        }

        public void SetFlashAmount(float amount)
        {
        }

        public void FlashColor(float flickerDuration, int flickerAmount)
        {
        }

        public void ClearFlashColor()
        {
        }
    }
}