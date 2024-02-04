using com.mec;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotateObject : MonoBehaviour, IRotate
{
    public virtual Stat Speed { set; get; }

    private CoroutineHandle handle;
    private Quaternion origin;

    private void Awake()
    {
        origin = this.transform.localRotation;
    }
    public virtual void Play()
    {
        if (handle.IsValid) Timing.KillCoroutines(handle);
        handle = Timing.RunCoroutine(_AutoRotate());
    }

    public void Pause()
    {
        Timing.PauseCoroutines(handle);
    }
    public void UnPause()
    {
        Timing.ResumeCoroutines(handle);
    }
    private void OnDisable()
    {
        this.transform.localRotation = origin;
        if (handle.IsValid) Timing.KillCoroutines(handle);
    }

    private IEnumerator<float> _AutoRotate()
    {
        while (true)
        {
            float v = Speed.Value * Time.deltaTime;
            var rotate = new Vector3(0, 0, v);
            this.transform.Rotate(rotate);
            yield return Timing.DeltaTime;
        }
    }

    public void PauseRotate()
    {
        Pause();
    }

    public void ResumeRotate()
    {
        UnPause();
    }
}