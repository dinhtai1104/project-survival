using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game
{
    public class Utility
    {
        public static async UniTask<string> GetData(string url)
        {
#if UNITY_EDITOR
            Logger.Log(url);
#endif
            UnityWebRequest webrequest = UnityWebRequest.Get(url);
            webrequest.timeout = 400;
            UnityWebRequestAsyncOperation sendWebRequestAsyncOperation = webrequest.SendWebRequest();
            await sendWebRequestAsyncOperation;
            return webrequest.downloadHandler.text;
        }
        public static Vector2 CalculateBelzierCurve(float t, Vector2 from, Vector2 middle, Vector2 target)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            Vector2 p = uu * from;
            p += 2 * u * t * middle;
            p += tt * target;
            return p;
        }
    }
}