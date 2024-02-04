using Game.SDK;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(SDKConfigData))]
public class SDKConfigEditor : Editor
{
    string json;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        SDKConfigData target = (SDKConfigData)this.target;
      
        if (GUILayout.Button("Create json"))
        {
            json=Newtonsoft.Json.JsonConvert.SerializeObject(target,new ObscuredValueConverter());
        }
        if (!string.IsNullOrEmpty(json))
        {
            GUILayout.TextArea(json, GUILayout.Height(100));
        }

       
    }
}
