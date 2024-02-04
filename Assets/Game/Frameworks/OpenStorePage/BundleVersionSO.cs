using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu]
public class BundleVersionSO : ScriptableObject
{
    public int BundleVersion;

    public void Save()
    {
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }

    private void OnValidate()
    {
#if UNITY_EDITOR
        Save();
#if UNITY_ANDROID
        BundleVersion = PlayerSettings.Android.bundleVersionCode;
#elif UNITY_IOS
        BundleVersion= PlayerSettings.iOS.buildNumber.TryGetInt();
#endif
        Logger.Log("VERSION CODE: " + BundleVersion);
        Save();

#endif
    }
}