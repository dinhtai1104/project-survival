using System.Collections.Generic;

public class UserProperties
{
    public Dictionary<string, object> Properties;
    public void AddProperties(string key, object value)
    {
        if (Properties == null) Properties = new Dictionary<string, object>();
        if (Properties.ContainsKey(key)) Properties[key] = value;
        else Properties.Add(key, value);
    }
}