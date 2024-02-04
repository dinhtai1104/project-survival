using Cysharp.Threading.Tasks;
using Game.GameActor;
using System.Collections.Generic;
using UnityEngine;

public class ElectricImpactStatusHandler : ImpactStatusHandler
{
    [SerializeField]
    private ValueConfigSearch Range,Damage;

    public override async UniTask Apply(ActorBase caster, ActorBase target, ImpactHandler handler)
    {
        Sensor sensor = GetComponent<Sensor>();
        sensor.detectRange = Range.FloatValue;
        var targets=sensor.SearchAll((Character)caster,transform.position);
        string[] damage = Damage.StringValue.Split(',');
        int index = 0;
        foreach(var t in targets)
        {
            var status = (await ((Character)t).StatusEngine.AddStatus(caster, EStatus.Shock, this));
            if (status != null)
            {
                status.Init(caster, (Character)t);
                //shock once and instantly
                status.SetDuration(0.1f);
                status.SetCooldown(0.08f);
                status.SetDmgMul(float.Parse(damage[index]));

                index++;


            }
        }
       
    }
}