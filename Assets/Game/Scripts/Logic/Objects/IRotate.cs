public interface IRotate
{
    Stat Speed { set; get; }

    void PauseRotate();
    void ResumeRotate();
    void Play();
}

public class NullRotate : IRotate
{
    public Stat Speed { set; get; }

    public void PauseRotate()
    {
    }

    public void Play()
    {
    }

    public void ResumeRotate()
    {
    }
}