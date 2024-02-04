using System;
using UnityEngine.UI;

namespace Framework.UI
{
    public class CanvasScalerProfile: BaseCanvasProfile<CanvasScalerProfile.AspectProfile>
    {
        [Serializable]
        public class AspectProfile : CanvasProfile
        {
            public CanvasScaler.ScreenMatchMode screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        }

        public CanvasScaler canvasScaler;

        public override void OnApplyProfile(AspectProfile profile)
        {
            canvasScaler.screenMatchMode = profile.screenMatchMode;
        }

#if UNITY_EDITOR
        private void Reset()
        {
            canvasScaler = GetComponent<CanvasScaler>();
        }
#endif
    }
}