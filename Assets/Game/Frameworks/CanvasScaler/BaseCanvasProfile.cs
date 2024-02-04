using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Framework.UI
{
    public abstract class BaseCanvasProfile<T> : MonoBehaviour where T : CanvasProfile, new()
    {
        [TableList] public List<T> profiles;

        public float CurrentScreenAspect
        {
            get
            {
#if UNITY_EDITOR
                var mainGameViewSize = GetMainGameViewSize();
                return mainGameViewSize.x / mainGameViewSize.y;
#endif
#pragma warning disable 162
                return (float)Screen.width / (float)Screen.height;
#pragma warning restore 162
            }
        }

        public T CurrentProfile => FindClosestScreenSize(CurrentScreenAspect);

        protected virtual void Start()
        {
            if (profiles != null && profiles.Count > 0) Fix();
        }



        [Button]
        public virtual void Fix()
        {
            var profile = FindClosestScreenSize(CurrentScreenAspect);
            OnApplyProfile(profile);
        }

        public abstract void OnApplyProfile(T profile);

        protected virtual T FindClosestScreenSize(float aspect)
        {
            return profiles.OrderBy(n => Math.Abs(n.aspectRatio - aspect)).First();
        }



#if UNITY_EDITOR


        private Vector2 GetMainGameViewSize()
        {
            System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
            System.Reflection.MethodInfo GetSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView",
                                                                             System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            System.Object Res = GetSizeOfMainGameView.Invoke(null, null);
            return (Vector2)Res;
        }

        private Vector2 resolution;

        private void Awake()
        {
            resolution = new Vector2(Screen.width, Screen.height);
        }

        private void LateUpdate()
        {
            if (Math.Abs(resolution.x - Screen.width) > Mathf.Epsilon ||
                Math.Abs(resolution.y - Screen.height) > Mathf.Epsilon)
            {
                resolution = new Vector2(Screen.width, Screen.height);
                Debug.Log("detect canvas change size. Auto change this GO: " + gameObject.name);
                Fix();
            }
        }

        [Button]
        public void CreateEmptyProfile()
        {
            profiles = new List<T>();
            foreach (var size in SupportScreenSize.ScreenSizes)
            {
                int x = (int)size.x;
                int y = (int)size.y;


                int aspectX = x / GCD(x, y);
                int aspectY = y / GCD(x, y);

                float aspectRatio = (float)x / (float)y;

                bool alreadyAdded = false;

                foreach (var profile in profiles)
                {
                    if (Math.Abs(profile.aspectRatio - aspectRatio) < Mathf.Epsilon)
                    {
                        alreadyAdded = true;
                        break;
                    }
                }

                if (!alreadyAdded)
                    profiles.Add(new T()
                    { name = $"{aspectX}:{aspectY}", aspectRatio = aspectRatio, screenSize = new Vector2(x, y) });
            }

            profiles = profiles.OrderBy(x => x.aspectRatio).ToList();
        }


#endif

        /// <summary> GreatestCommonDivisor </summary>
        public static int GCD(int a, int b)
        {
            int Remainder;

            while (b != 0)
            {
                Remainder = a % b;
                a = b;
                b = Remainder;
            }

            return a;
        }

    }
}