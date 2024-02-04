using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class UIIntroBossPanel : UI.Panel
{
    [SerializeField]
    private Spine.Unity.SkeletonGraphic bossAnim;
    [SerializeField]
    private AnimData bossData;
    [SerializeField]
    private TMPro.TextMeshProUGUI titleText, subTitleText;

    [SerializeField]
    private string test="Boss@Stage@10";
    public override void PostInit()
    {
    }
    [Button]
    public void Test()
    {
        SetUp(test);
    }
    public void SetUp(string enemyId)
    {
        Time.timeScale = 0;
        var data = bossData.Get(enemyId);
        titleText.SetText(data.title);
        subTitleText.SetText(data.subTitle);
        Show();
        //bossAnim.AnimationState.SetAnimation(0, "idle", true);

    }

    public void SetAnim(string anim)
    {
        bossAnim.AnimationState.SetAnimation(0, anim, false);
    }
    public void SetAnimLoop(string anim)
    {
        bossAnim.AnimationState.SetAnimation(0, anim, true);
    }

    public void AddAnimation(string anim)
    {
        bossAnim.AnimationState.AddAnimation(0, anim, false, 0);
    }
    public void AddAnimationLoop(string anim)
    {
        bossAnim.AnimationState.AddAnimation(0, anim, true, 0);
    }

    public void TriggerClose()
    {
        Game.Transitioner.Controller.Instance.Trigger(Color.black).ContinueWith(() =>
        {
            Time.timeScale = 1;

            Deactive();
            Game.Transitioner.Controller.Instance.Release();
        }).Forget() ;
    }
}
