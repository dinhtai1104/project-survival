using UnityEngine;

namespace Framework.UI
{
    public static class SupportScreenSize
    {
        public static readonly Vector2[] ScreenSizes = new[]
        {
            new Vector2(1280, 720), new Vector2(1920, 1080), new Vector2(800,  480), new Vector2(854,  480),
            new Vector2(960,  540), new Vector2(1024, 600), new Vector2(1280,  800), new Vector2(2560, 1440),
            new Vector2(480,  320), new Vector2(1920, 1200), new Vector2(1024, 768),new Vector2(2960, 1440),new Vector2(2160, 1080), new Vector2(1024,600),
            new Vector2(1280,800),
        };
    }
}