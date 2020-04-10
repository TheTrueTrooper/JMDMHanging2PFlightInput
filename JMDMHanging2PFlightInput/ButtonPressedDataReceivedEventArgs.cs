using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JMDMHanging2PFlight
{
    public class ButtonPressedDataReceivedEventArgs : EventArgs
    {
        public char Button { get; private set; }

        internal ButtonPressedDataReceivedEventArgs(char Button)
        {
            this.Button = Button;
        }
    }
}
