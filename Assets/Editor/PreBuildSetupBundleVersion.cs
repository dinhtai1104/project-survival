using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Callbacks;
using UnityEngine;

public class PreBuildSetupBundleVersion
{
    [PostProcessBuildAttribute(999)]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string pathToBuildProject)
    {
        //return;
        if (Application.platform == RuntimePlatform.Android)
        {
            var scriptableSO = AssetDatabaseUtils.GetAssetOfType<BundleVersionSO>("AndroidBundleVersionSO");
            scriptableSO.BundleVersion = PlayerSettings.Android.bundleVersionCode;
            scriptableSO.Save();
            Debug.Log("SET BUNDLE: " + scriptableSO.BundleVersion);
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            var scriptableSO = AssetDatabaseUtils.GetAssetOfType<BundleVersionSO>("iOSBundleVersionSO");
            scriptableSO.BundleVersion = PlayerSettings.iOS.buildNumber.TryGetInt();
            scriptableSO.Save();
            Debug.Log("SET BUNDLE: " + scriptableSO.BundleVersion);
        }
    }
    private static void BuildPlayerHandler(BuildPlayerOptions buildPlayerOptions)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            var scriptableSO = AssetDatabaseUtils.GetAssetOfType<BundleVersionSO>("AndroidBundleVersionSO");
            scriptableSO.BundleVersion = PlayerSettings.Android.bundleVersionCode;
            scriptableSO.Save();
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            var scriptableSO = AssetDatabaseUtils.GetAssetOfType<BundleVersionSO>("iOSBundleVersionSO");
            scriptableSO.BundleVersion = PlayerSettings.iOS.buildNumber.TryGetInt();
            scriptableSO.Save();
        }
    }
}
