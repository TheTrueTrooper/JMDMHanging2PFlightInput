using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JMDMHanging2PFlight
{
    public class JMDMHanging2PFlightInput_Com : IDisposable
    {
        public delegate void ButtonPressReceivedEventHandler(object sender, ButtonPressedDataReceivedEventArgs e);

        public event ButtonPressReceivedEventHandler ButtonPressReceivedEvent;

        internal SerialPort InputListenCom { set; get; }

        internal string ListenPort { private set; get; }

        bool Disposed = false;

        Thread ListenerThread;

        internal bool IsOpen
        {
            get => InputListenCom.IsOpen;
        }

        public JMDMHanging2PFlightInput_Com(string ListenPort)
        {
            this.ListenPort = ListenPort;
            InputListenCom = new SerialPort(ListenPort, 9600, Parity.None, 8, StopBits.One);
            InputListenCom.ReadTimeout = -1;

        }

        public void OpenPortAndListen()
        {
            if (!InputListenCom.IsOpen)
            {
                InputListenCom.Open();
                ListenerThread = new Thread(new ParameterizedThreadStart(Listening));
                ListenerThread.Start(this);
            }
        }

        void Listening(object ThisIn)
        {
            JMDMHanging2PFlightInput_Com This = (JMDMHanging2PFlightInput_Com)ThisIn;
            bool Disposed = false;
            lock (This)
                Disposed = This.Disposed;
            while (!Disposed)
            {
                //acuire a message
                //start the message
                char[] MessageType = new char[14];
                lock (This)
                    MessageType[0] = (char)This.InputListenCom.ReadByte();
                int Count = 1;
                //then read bytes until message done
                do
                {
                    lock (This)
                        MessageType[Count] = (char)This.InputListenCom.ReadByte();
                    Count++;
                }
                while (MessageType[Count - 1] != '\0' && Count < 14);
                //Dispach message
                if (MessageType[0] == 'G' && MessageType[1] == 'A' && MessageType[2] == 'T' && MessageType[3] == '\0')
                    ButtonPressReceivedEvent.Invoke(this, new ButtonPressedDataReceivedEventArgs('A'));
                else if (MessageType[0] == 'G' && MessageType[1] == 'B' && MessageType[2] == 'T' && MessageType[3] == '\0')
                    ButtonPressReceivedEvent.Invoke(this, new ButtonPressedDataReceivedEventArgs('B'));
                else if (MessageType[0] == 'G' && MessageType[1] == 'C' && MessageType[2] == 'T' && MessageType[3] == '\0')
                    ButtonPressReceivedEvent.Invoke(this, new ButtonPressedDataReceivedEventArgs('C'));
                else if (MessageType[0] == 'G' && MessageType[1] == 'D' && MessageType[2] == 'T' && MessageType[3] == '\0')
                    ButtonPressReceivedEvent.Invoke(this, new ButtonPressedDataReceivedEventArgs('D'));
                lock (This)
                    Disposed = This.Disposed;
            }
        }

        public void Dispose()
        {
            if (!Disposed)
            {
                Disposed = true;
                ListenerThread.Abort();
                InputListenCom.Dispose();
            }
        }

        ~JMDMHanging2PFlightInput_Com()
        {
            if (!Disposed)
            {
                Disposed = true;
                ListenerThread?.Abort();
                InputListenCom?.Dispose();
            }
        }
    }
}