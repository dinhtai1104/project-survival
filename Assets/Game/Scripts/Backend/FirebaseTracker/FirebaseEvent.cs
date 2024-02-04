using Firebase.Analytics;
using Sirenix.Utilities;
using System.Collections.Generic;
using System.Linq;

public class FirebaseEvent : IEvent
{
    private readonly string _id;
    private readonly List<Parameter> _parameters;

#if UNITY_EDITOR
    private Dictionary<string, object> _debugParameter;
#endif
    public FirebaseEvent(string id)
    {
        _id = id;
        _parameters = new List<Parameter>();
#if UNITY_EDITOR
        _debugParameter = new Dictionary<string, object>();
#endif
    }

    public IEvent AddStringParam(string name, string value)
    {
        name = name.Trim();
#if UNITY_EDITOR
        _debugParameter.Add(name, value);
        return this;
#endif
        _parameters.Add(new Parameter(name, value));
        return this;
    }

    public IEvent AddIntParam(string name, int value)
    {
        try
        {
            name = name.Trim();
#if UNITY_EDITOR
            _debugParameter.Add(name, value);
            return this;
#endif
            _parameters.Add(new Parameter(name, value));
            return this;
        }
        catch (System.Exception e)
        {
        }
        return this;
    }

    public IEvent AddFloatParam(string name, float value)
    {
        name = name.Trim();
        try
        {
#if UNITY_EDITOR
            _debugParameter.Add(name, value);
            return this;
#endif

            _parameters.Add(new Parameter(name, value));
            return this;
        }
        catch (System.Exception e)
        {
        }
        return this;
    }

    public void Track()
    {
#if UNITY_EDITOR
        var data = "";
        _debugParameter.ForEach(e => data += e.Key + ";" + e.Value + " , ");
        Logger.Log(">>TRACKING: " + _id + " " + data);
#endif
        try
        {
            FirebaseAnalysticController.Instance.SetUserProperties();
            FirebaseAnalytics.LogEvent(_id, _parameters.ToArray());
        }
        catch (System.Exception e)
        {

        }
    }

    public IEvent AddDoubleParam(string name, double value)
    {
        name = name.Trim();
        try
        {
#if UNITY_EDITOR
            _debugParameter.Add(name, value);
            return this;
#endif

            _parameters.Add(new Parameter(name, value));
            return this;
        }
        catch (System.Exception e)
        {

        }
        return this;
    }
}