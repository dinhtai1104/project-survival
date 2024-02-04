public interface IInputHandler
{
    void Initialize();
    void Ticks();

    void SetActive(bool active);
}