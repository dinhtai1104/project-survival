using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace Engine
{
    public class UIHealthHud : MonoBehaviour
    {
        [SerializeField] private PositionConstraint m_Constraint;

        private Actor m_Owner;

        public Actor Owner => m_Owner;
        public PositionConstraint Constraint => m_Constraint;

        public virtual void SetOwner(Actor actor)
        {
            m_Owner = actor;
        }

        public virtual void SetParentConstraint(Transform sourceTransform)
        {
            m_Constraint.constraintActive = false;
            var source = new ConstraintSource { weight = 1f, sourceTransform = sourceTransform };
            m_Constraint.AddSource(source);
            m_Constraint.constraintActive = true;
        }

        protected virtual void OnDisable()
        {
            for (var i = 0; i < Constraint.sourceCount; i++)
            {
                Constraint.RemoveSource(i);
            }
        }
    }
}