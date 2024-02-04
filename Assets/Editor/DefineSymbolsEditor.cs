using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class DefineSymbolsEditor : EditorWindow
{
    private readonly string[] Symbols =
    {
        "UNITASK_DOTWEEN_SUPPORT",
        "DEVELOPMENT",    
        "DEVELOPMENT2",    
        "PRODUCTION",
        "PREBUILD_BUNDLE",
        "TEST_HERO2",
        "IAP_PRODUCTION",
        "IAP_DEVELOPMENT",
    };

    [MenuItem("Tools/Define Symbols Editor")]
    public static void OpenWindow()
    {
        EditorWindow window = EditorWindow.GetWindow(typeof(DefineSymbolsEditor));
        window.minSize = new Vector2(320, 320);
        window.maxSize = new Vector2(320, 640);
    }

    private void HorizontalLine()
    {
        var horizontalLine = new GUIStyle();
        horizontalLine.normal.background = EditorGUIUtility.whiteTexture;
        horizontalLine.margin = new RectOffset(4, 4, 4, 4);
        horizontalLine.fixedHeight = 1;

        var c = GUI.color;
        GUI.color = Color.grey;
        GUILayout.Box(GUIContent.none, horizontalLine);
        GUI.color = c;
    }

    private void OnGUI()
    {
        HorizontalLine();
        foreach (var s in Symbols)
        {
            RenderTable(s);
        }

        HorizontalLine();
        RenderAddAllSymbols();
        HorizontalLine();
    }

    private void RenderTable(string symbols)
    {
        EditorGUILayout.BeginHorizontal();
        GUI.enabled = !IsContains(symbols);
        if (GUILayout.Button("Add", GUILayout.Height(15), GUILayout.Width(40)))
        {
            AddSymbols(symbols);
        }

        GUI.enabled = true;

        GUI.enabled = IsContains(symbols);
        if (GUILayout.Button("Remove", GUILayout.Height(15), GUILayout.Width(60)))
        {
            RemoveSymbols(symbols);
        }

        GUI.enabled = true;

        EditorGUILayout.LabelField(symbols, GUILayout.MaxWidth(position.width / 2));
        EditorGUILayout.EndHorizontal();
    }

    private void RenderAddAllSymbols()
    {
        EditorGUILayout.BeginVertical();
        if (GUILayout.Button("Add All"))
        {
            foreach (var s in Symbols)
            {
                AddSymbols(s);
            }
        }

        EditorGUILayout.EndVertical();
    }

    private void AddSymbols(string symbols)
    {
        var definesString =
            PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
        var allDefines = definesString.Split(';').ToList();
        if (!allDefines.Contains(symbols))
        {
            allDefines.Add(symbols);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(
                EditorUserBuildSettings.selectedBuildTargetGroup, string.Join(";", allDefines.ToArray()));
        }
    }

    private void RemoveSymbols(string symbols)
    {
        var definesString =
            PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
        var allDefines = definesString.Split(';').ToList();
        if (allDefines.Contains(symbols))
        {
            foreach (var item in allDefines)
            {
                if (item == symbols)
                {
                    allDefines.Remove(item);
                    break;
                }
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(
                EditorUserBuildSettings.selectedBuildTargetGroup, string.Join(";", allDefines.ToArray()));
        }
    }

    private bool IsContains(string symbols)
    {
        var definesString =
            PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
        var allDefines = definesString.Split(';').ToList();
        return allDefines.Contains(symbols);
    }
}