using Cysharp.Threading.Tasks;

public class SetWeaponTask : SkillTask
{
    public int weapon = 0;
    public override async UniTask Begin()
    {
        await base.Begin();
        Caster.WeaponHandler.SetWeapon(weapon);
        IsCompleted = true;
    }
}
