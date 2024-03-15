using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace Engine
{
    public class SpineGraphic : MonoBehaviour, IGraphicEngine
    {
        [SerializeField] private bool m_GraphicAlphaOnStart;
        [SerializeField, Range(0, 1f)] private float m_GraphicAlpha = 1f;

        private const string ShaderFillColor = "_FillColor";
        private const string ShaderFillPhase = "_FillPhase";

        private static readonly int Color = Shader.PropertyToID(ShaderFillColor);
        private static readonly int FlashAmount = Shader.PropertyToID(ShaderFillPhase);

        private SkeletonAnimation[] m_Animations;
        private Renderer[] m_Renderers;
        private MaterialPropertyBlock m_PropBlock;
        private Tweener m_FlashColorTween;

        public ActorBase Owner { get; private set; }

        private void Awake()
        {
            m_Animations = GetComponentsInChildren<SkeletonAnimation>();
            m_Renderers = GetComponentsInChildren<Renderer>();
        }

        private void Start()
        {
            if (m_GraphicAlphaOnStart)
            {
                SetGraphicAlpha(m_GraphicAlpha);
            }
        }

        public void Init(ActorBase actor)
        {
            Owner = actor;
            m_PropBlock = new MaterialPropertyBlock();
        }

        public void SetActiveRenderer(bool active)
        {
            foreach (var r in m_Renderers)
            {
                r.enabled = active;
            }
        }

        public void SetGraphicAlpha(float a)
        {
            foreach (var anim in m_Animations)
            {
                anim.Skeleton.A = a;
            }
        }

        public void SetFlashAmount(float amount)
        {
            foreach (var r in m_Renderers)
            {
                r.GetPropertyBlock(m_PropBlock);
            }

            m_PropBlock.SetFloat(FlashAmount, amount);

            foreach (var r in m_Renderers)
            {
                r.SetPropertyBlock(m_PropBlock);
            }
        }

        public void FlashColor(float flickerDuration, int flickerAmount)
        {
            m_FlashColorTween?.Kill();
            m_FlashColorTween = DOTween.To(SetFlashAmount, 0f, 1f, flickerDuration)
                .OnComplete(() => { SetFlashAmount(0f); }).SetLoops(flickerAmount);
        }

        public void ClearFlashColor()
        {
            SetFlashAmount(0f);
        }
    }
}