using Game.GameActor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Logic.Objects
{
    public class BulletSimpleDamageObjectHasMiniBullet : BulletSimpleDamageObject
    {
        public override Stat DmgStat
        {
            set
            {
                base.DmgStat = value;
                foreach (var bl in miniBullets)
                {
                    bl.DmgStat = new Stat(DmgStat.Value * dmgMiniBullet.FloatValue);
                }
            }
        }
        public BulletSimpleDamageObject[] miniBullets;
        public ValueConfigSearch dmgMiniBullet;
        public override void SetMaxHit(int maxHit)
        {
            base.SetMaxHit(maxHit);
            foreach (var bl in miniBullets)
            {
                bl.SetMaxHit(maxHit);
            }
        }
        public override void SetCaster(ActorBase caster)
        {
            base.SetCaster(caster);
            foreach (var bl in miniBullets)
            {
                bl.SetCaster(caster);
            }
        }
        public override void Play()
        {
            base.Play();
            foreach (var bl in miniBullets)
            {
                bl.gameObject.SetActive(true);
            }
        }
    }
}
