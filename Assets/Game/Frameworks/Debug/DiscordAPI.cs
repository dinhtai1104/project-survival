using System;
using UnityEngine.Networking;
using UnityEngine;
using System.Collections;
using System.Text;
using com.mec;

namespace com.debug.log
{
    public class DiscordAPI
    {
        // change webhook token for chanel
        private const string _apiWebhook = "1162580299876671599/SkU2K2TnIT22kbAbxBrvup4V1QbwFSi7vtgJRrCg4aauW6CDT-pGJTZtCm8G48xYZEEt";
        private const string _baseUri = "https://discord.com/api/webhooks/";
        private const string _postMessageUri = _baseUri + _apiWebhook;

        public IEnumerator PostMessage(string message, Action onSuccess = null,
            Action<string> onError = null)
        {
            yield return new WaitForSeconds(1f);
            var form = new WWWForm();
            form.AddField("content", GenerateMessage(message));
            var coPost = CoPost(_postMessageUri, form, onSuccess, onError);
            while (coPost.MoveNext())
            {
                yield return null;
            }
        }
        private IEnumerator CoPost(string uri, WWWForm form, Action onSuccess = null, Action<string> onError = null)
        {
            yield return new WaitForSeconds(1f);
#if UNITY_2017_1_OR_NEWER
            var www = UnityWebRequest.Post(uri, form);
            var operation = www.SendWebRequest();
            while (!operation.isDone)
#else
			var www = new WWW(uri, form);
			while (!www.isDone)
#endif
            {
                yield return null;
            }

            var error = www.error;
            if (!string.IsNullOrEmpty(error))
            {
                if (onError != null)
                {
                    onError(error);
                }

                yield break;
            }

            if (onSuccess != null)
            {
                onSuccess();
            }

            www.Dispose();
        }

        private string GenerateMessage(string logMessage)
        {
            // var byteToM = 0.000001f;
            var sb = new System.Text.StringBuilder();
            // var now = DateTime.Now;
            // sb.Append("*----SystemInfo----*\n");
            sb.AppendFormat("*{0} | {1}*\n", Application.companyName, Application.productName);
            sb.AppendFormat("{0:dd/MM/yyyy, HH:mm:ss} | {1}\n", DateTime.Now, SystemInfo.deviceName);
            // sb.AppendFormat("OS: {0} | {1}\n", SystemInfo.operatingSystem, SystemInfo.operatingSystemFamily);
            // sb.AppendFormat("Graphics: {0} | {1} | {2} | Memory: {3}\n", SystemInfo.graphicsDeviceName,
            //     SystemInfo.graphicsDeviceType, SystemInfo.graphicsDeviceVersion, SystemInfo.graphicsMemorySize);
            // sb.AppendFormat("Processor: {0} Core: {1} {2}MHz\n", SystemInfo.processorType, SystemInfo.processorCount,
            //     SystemInfo.processorFrequency);
            // sb.AppendFormat("Battery: {0}% {1}\n", SystemInfo.batteryLevel * 100f, SystemInfo.batteryStatus);
            // sb.AppendFormat("Memory: {0}\n", SystemInfo.systemMemorySize);
            // sb.AppendFormat("Alloc Memory : {0}M\n", Profiler.GetTotalAllocatedMemoryLong() * byteToM);
            // sb.AppendFormat("Reserved Memory : {0}M\n", Profiler.GetTotalReservedMemoryLong() * byteToM);
            // sb.AppendFormat("Unused Reserved: {0}M\n", Profiler.GetTotalUnusedReservedMemoryLong() * byteToM);
            // sb.AppendFormat("Mono Heap : {0}M\n", Profiler.GetMonoHeapSizeLong() * byteToM);
            // sb.AppendFormat("Mono Used : {0}M\n", Profiler.GetMonoUsedSizeLong() * byteToM);

            // sb.Append("*----SceneInfo----*\n");
            // for (var i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
            // {
            //     sb.AppendFormat("{0}\n", UnityEngine.SceneManagement.SceneManager.GetSceneAt(i).name);
            // }

            // sb.Append("*----Log----*\n");
            sb.AppendFormat("```{0}```\n", logMessage);
            return sb.ToString();
        }
    }
}
