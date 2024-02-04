using Cysharp.Threading.Tasks;
using Game.GameActor;
using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBlock : MonoBehaviour
{
    bool isActive = false;
    public ValueConfigSearch SlowRate;
    public ValueConfigSearch TimeSlow;
    public ECharacterType targetType;
    public AnimationCurve moveCurve;

    public MMF_Player activeFB, deactiveFB;
    Character mainActor;
    float time = 99;
    float slowRate = 0;
    public void Trigger()
    {
        mainActor = (Character)Game.Controller.Instance.gameController.GetMainActor();
        slowRate = SlowRate.FloatValue;
        time = TimeSlow.FloatValue ;
        SetTime(slowRate);
        Messenger.Broadcast<bool>(EventKey.FreezeTime, true);

        isActive = true;
        activeFB?.PlayFeedbacks();
    }

    public void Release()
    {
        SetTime(1);
        Messenger.Broadcast<bool>(EventKey.FreezeTime, false);
        isActive = false;

        GetComponentInChildren<SpriteRenderer>().color = Color.grey;
        deactiveFB?.PlayFeedbacks();
    }

    async UniTask SetTime(float scale)
    {
        float time = 0;
        float duration = moveCurve.keys[moveCurve.length - 1].time;
        float from = GameTime.Controller.TIME_SCALE;
        while (time < duration)
        {
            GameTime.Controller.TIME_SCALE = from + moveCurve.Evaluate(time) * (scale - from);

            time += Time.deltaTime;
            await UniTask.Yield();
        }
        GameTime.Controller.TIME_SCALE = scale;

    }
   
    private void Update()
    {
        if (!isActive) return;
        if (time > 0)
        {
            time -= Time.unscaledDeltaTime;
        }
        else
        {
            Release();
        }
    }
    
}
