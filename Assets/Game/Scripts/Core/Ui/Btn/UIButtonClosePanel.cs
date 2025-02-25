using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ui.View;

namespace Ui.Btn
{
    public class UIButtonClosePanel : UIBaseButton
    {
        public override void Action()
        {
            GetComponentInParent<Panel>().Close();
        } 
    }
}
