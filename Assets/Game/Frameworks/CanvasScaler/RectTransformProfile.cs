using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Framework.UI
{
    public class RectTransformProfile : BaseCanvasProfile<RectTransformProfile.Profile>
    {
        [Serializable]
        public class Profile : CanvasProfile
        {
            public Vector3 localPos;
        }

        public RectTransform myRectTransform;

        public override void OnApplyProfile(Profile profile)
        {
            myRectTransform.localPosition = profile.localPos;
        }

#if UNITY_EDITOR

        private void Reset()
        {
            myRectTransform = GetComponent<RectTransform>();
        }

        [Button]
        private void UseCurrentPosition()
        {
            var profile = CurrentProfile;
            profile.localPos = myRectTransform.localPosition;
        }

#endif
    }
}