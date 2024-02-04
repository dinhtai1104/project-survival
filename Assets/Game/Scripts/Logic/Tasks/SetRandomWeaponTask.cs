using Cysharp.Threading.Tasks;

public class SetRandomWeaponTask : SkillTask
{
    public int[] ids;
    public override async UniTask Begin()
    {
        await base.Begin();
        Caster.WeaponHandler.SetWeapon(ids[UnityEngine.Random.Range(0,ids.Length)]);
        IsCompleted = true;
    }
}