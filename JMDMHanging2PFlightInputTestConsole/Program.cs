using JMDMHanging2PFlight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JMDMHanging2PFlightInputTestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            JMDMHanging2PFlightInput_Com Com = new JMDMHanging2PFlightInput_Com("COM3");
            Com.ButtonPressReceivedEvent += (object This, ButtonPressedDataReceivedEventArgs e) => { Console.WriteLine($"ButtonRecived:{e.Button}"); };

            Com.OpenPortAndListen();
            Console.ReadKey();
        }
    }
}
