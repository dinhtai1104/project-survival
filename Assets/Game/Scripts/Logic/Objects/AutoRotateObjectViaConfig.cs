using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Logic.Objects
{
    public class AutoRotateObjectViaConfig : AutoRotateObject
    {
        public ValueConfigSearch speedConfig;

        private void OnEnable()
        {
            Play();
        }

        public override void Play()
        {
            if (Speed == null)
            {
                Speed = new Stat(speedConfig.FloatValue);
            }
            else
            {
                Speed.ClearModifiers();
                Speed.AddModifier(new StatModifier(EStatMod.Flat, speedConfig.FloatValue));
            }
            base.Play();
        }
    }
}
