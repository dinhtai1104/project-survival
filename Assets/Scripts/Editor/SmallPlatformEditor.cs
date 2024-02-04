using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(SmallPlatform), true), CanEditMultipleObjects]
public class SmallPlatformEditor : Editor
{
    private void OnEnable()
    {
        width = ((SmallPlatform)target).mid.GetComponent<SpriteRenderer>().size.x.ToString();
    }
    string width="0";
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Label("Width: "+width);
        width = GUILayout.TextField(width.ToString(),GUILayout.Width(200),GUILayout.Height(20));
        if (GUILayout.Button("APPLY"))
        {
            Logger.Log("WIDTH:" + width);
            SmallPlatform smallPlatform = (SmallPlatform)target;
            smallPlatform.Apply(float.Parse(width));
        }
    }
}
