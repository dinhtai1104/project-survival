using System;
using UnityEngine;

[System.Serializable]
public class ValueConfigSearch
{
    public string ConfigKey;

    public ValueConfigSearch(string configKey)
    {
        ConfigKey = configKey;
    }
    public ValueConfigSearch(string configKey,string defaultValue)
    {
        ConfigKey = configKey;
        DefaultValue = defaultValue;
    }

    public ValueConfigSearch()
    {
    }

    protected string ConfigValueRaw {
        get
        {
            try
            {
                return DataManager.Base.GeneralConfig.GetValue(ConfigKey, DefaultValue);
            }
            catch (Exception e)
            {
                return DefaultValue;
            }
        }
    }
    public string DefaultValue;

    public ValueConfigSearch SetId(string id)
    {
        if (string.IsNullOrEmpty(ConfigKey)) return this;
        ConfigKey = string.Format(ConfigKey, id);
        return this;
    }
    public ValueConfigSearch Clone()
    {
        return new ValueConfigSearch(ConfigKey,DefaultValue);
    }


    public int IntValue => string.IsNullOrEmpty(ConfigValueRaw)? (string.IsNullOrEmpty(DefaultValue) ? 0 : int.Parse(DefaultValue)):ConfigValueRaw.TryGetInt();
    public float FloatValue => string.IsNullOrEmpty(ConfigValueRaw) ? (string.IsNullOrEmpty(DefaultValue)?0:float.Parse(DefaultValue)) : ConfigValueRaw.TryGetFloat();
    public Vector2 Vector2Value => string.IsNullOrEmpty(ConfigValueRaw) ? GetVector2(ConfigValueRaw) : ConfigValueRaw.TryGetVector2();
    public Vector3 Vector3Value => string.IsNullOrEmpty(ConfigValueRaw) ? GetVector3(ConfigValueRaw) : ConfigValueRaw.TryGetVector3();
    public string StringValue => string.IsNullOrEmpty(ConfigValueRaw) ? "" : ConfigValueRaw;

    private Vector2 GetVector2(string value)
    {
        string[] splits = value.Split(';');
        return new Vector2(float.Parse(splits[0]),float.Parse(splits[1]));
    }
    private Vector3 GetVector3(string value)
    {
        string[] splits = value.Split(';');
        return new Vector3(float.Parse(splits[0]), float.Parse(splits[1]), float.Parse(splits[2]));
    }
}