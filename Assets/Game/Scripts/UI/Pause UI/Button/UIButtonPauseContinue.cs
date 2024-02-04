using UnityEngine;

public class UIButtonPauseContinue : UIBaseButton
{
    [SerializeField] private UIPausePanel panel;
    public override void Action()
    {
        panel.Close();
        Time.timeScale = 1;
    }
}
