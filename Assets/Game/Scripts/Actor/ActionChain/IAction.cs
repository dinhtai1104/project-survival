public interface IAction<T>
{
    void Begin(T owner);
    void End(T owner);
}