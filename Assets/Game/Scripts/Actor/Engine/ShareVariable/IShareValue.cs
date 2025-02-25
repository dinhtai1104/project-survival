namespace Engine
{
    public interface IShareValue
    {
        string Key { get; }
    }
    public interface IShareValue<T> : IShareValue
    {
        T Value { get; set; }
    }
}