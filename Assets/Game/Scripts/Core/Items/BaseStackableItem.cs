using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework
{
    [System.Serializable]
    public abstract class BaseStackableItem : BaseRuntimeItem
    {
        public int Capacity { get; }
        public int Amount => m_Value.TotalAmount;
        public bool Empty => Amount == 0;

        private readonly ResourceValue m_Value;

        public BaseStackableItem()
        {
            m_Value = new ResourceValue();
        }

        public BaseStackableItem(int capacity) : base()
        {
            Capacity = capacity;
        }
    }
}
