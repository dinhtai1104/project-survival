using Foundation.Game.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Core.Data.Datasave
{
    [System.Serializable]
    public class IAPSave
    {
        public string package;
        public DateTime date;

        public static IAPSave Buy(string product)
        {
            return new IAPSave { date = UnbiasedTime.UtcNow, package = product };
        }
    }
}
