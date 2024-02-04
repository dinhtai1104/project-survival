using System.Collections.Generic;

public interface IEvent
{
    IEvent AddIntParam(string name, int value);
    IEvent AddStringParam(string name, string value);
    IEvent AddFloatParam(string name, float value);
    IEvent AddDoubleParam(string name, double value);
    void Track();
}