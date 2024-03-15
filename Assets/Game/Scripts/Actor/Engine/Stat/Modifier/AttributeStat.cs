using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class AttributeStat
    {
        public string AttributeName;
        public Stat Stat;

        public AttributeStat(string attributeName, Stat stat)
        {
            AttributeName = attributeName;
            Stat = stat;
        }
    }
}
