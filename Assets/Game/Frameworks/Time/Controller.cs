using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameTime
{
    public class Controller : LiveSingleton<Controller>
    {
        public static float TIME_SCALE=1;

        public static float FixedDeltaTime(bool unscaleTime = false)
        {
            return (Time.fixedDeltaTime * (unscaleTime ? 1 : TIME_SCALE)*Time.timeScale);
        }
        public static float DeltaTime(bool unscaleTime = false)
        {
            return (Time.deltaTime * (unscaleTime ? 1 : TIME_SCALE)) * Time.timeScale;
        }
    }
   
}