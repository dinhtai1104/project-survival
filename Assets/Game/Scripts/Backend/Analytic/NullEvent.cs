public class NullEvent : IEvent
{
    public IEvent AddDoubleParam(string name, double value)
    {
        return this;
    }

    public IEvent AddFloatParam(string name, float value)
    {
        return this;
    }

    public IEvent AddIntParam(string name, int value)
    {
        return this;
    }

    public IEvent AddStringParam(string name, string value)
    {
        return this;
    }

    public void Track()
    {
    }
}