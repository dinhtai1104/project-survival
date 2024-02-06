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
    }
}
