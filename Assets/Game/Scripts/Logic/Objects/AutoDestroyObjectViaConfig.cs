public class AutoDestroyObjectViaConfig : AutoDestroyObject
{
    public ValueConfigSearch timeDestroy;
    protected override float TimeDestroy => timeDestroy.FloatValue;

    protected override void OnEnable()
    {
        var caster = GetComponent<CharacterObjectBase>();
        timeDestroy.SetId(caster.gameObject.name);
        base.OnEnable();
    }
}