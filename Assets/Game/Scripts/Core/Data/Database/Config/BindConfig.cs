using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class BindConfig
{
    public string ConfigKey;
    public string DefaultValue;

    public BindConfig(string configKey, object defaultValue)
    {
        ConfigKey = configKey;
        DefaultValue = defaultValue.ToString();
    }

    protected string ConfigValueRaw => DataManager.Base.GameConfig.GetValue(ConfigKey, DefaultValue);

    public void SetId(string id)
    {
        if (string.IsNullOrEmpty(ConfigKey)) return;
        ConfigKey = string.Format(ConfigKey, id);
    }

    public BindConfig Clone()
    {
        return new BindConfig(ConfigKey, DefaultValue);
    }

    // Binding
    public int IntValue => string.IsNullOrEmpty(ConfigValueRaw) ? (string.IsNullOrEmpty(DefaultValue) ? 0 : int.Parse(DefaultValue)) : int.Parse(ConfigValueRaw);
    public float FloatValue => string.IsNullOrEmpty(ConfigValueRaw) ? (string.IsNullOrEmpty(DefaultValue) ? 0 : float.Parse(DefaultValue)) : float.Parse(ConfigValueRaw);
    public Vector2 Vector2Value => string.IsNullOrEmpty(ConfigValueRaw) ? GetVector2(DefaultValue) : GetVector2(ConfigValueRaw);
    public Vector3 Vector3Value => string.IsNullOrEmpty(ConfigValueRaw) ? GetVector3(DefaultValue) : GetVector3(ConfigValueRaw);
    public string StringValue => string.IsNullOrEmpty(ConfigValueRaw) ? "" : ConfigValueRaw;
    public List<float> FloatsArray => string.IsNullOrEmpty(ConfigValueRaw) ? (string.IsNullOrEmpty(DefaultValue) ? new List<float>() : GetList(DefaultValue)) : GetList(ConfigValueRaw);
    private Vector2 GetVector2(string value)
    {
        string[] splits = value.Split(';');
        return new Vector2(float.Parse(splits[0]), float.Parse(splits[1]));
    }
    private Vector3 GetVector3(string value)
    {
        string[] splits = value.Split(';');
        return new Vector3(float.Parse(splits[0]), float.Parse(splits[1]), float.Parse(splits[2]));
    }

    private List<float> GetList(string value)
    {
        string[] splits = value.Split(';');
        var list = new List<float>();
        foreach (var i in splits)
        {
            list.Add(float.Parse(i));
        }
        return list;
    }
}
