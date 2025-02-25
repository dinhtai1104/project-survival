using Engine;
using ExtensionKit;
using UnityEngine;

namespace Assets.Game.Scripts.Tasks
{
    public class PlayUnityAnimationTask : Task
    {
        [SerializeField] private Animator m_Controller;
        [SerializeField] private string m_AnimationPlay;
        public override void Begin()
        {
            base.Begin();
            if (m_AnimationPlay.IsNotNullAndEmpty())
            {
                m_Controller.Play(m_AnimationPlay);
            }

            IsCompleted = true;
        }
    }
}
