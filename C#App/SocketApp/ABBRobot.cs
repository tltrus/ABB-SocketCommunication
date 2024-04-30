using ABB.Robotics.Controllers;
using ABB.Robotics.Controllers.RapidDomain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Windows;

namespace Painting
{
    internal class ABBRobot
    {
        public Controller SelectedController { get; set;  }
        private TcpClient TcpClient;
        private NetworkStream ClientStream;
        private int PORT = 4000;
        private string IP;

        public delegate void MessageHandler(string message);
        public event MessageHandler SendMessageEvent;
        public event MessageHandler GetMessageEvent;

        public ABBRobot()
        {

        }

        public void Connect(ControllerInfo controllerInfo)
        {
            if (controllerInfo is null) return;

            SelectedController = Controller.Connect(controllerInfo, ConnectionType.Standalone);
            SelectedController.Logon(UserInfo.DefaultUser);

            IP = SelectedController.IPAddress.ToString();
        }

        public void SendMessage(string message)
        {
            if (ClientStream is null) return;

            byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(message);
            ClientStream.Write(bytesToSend, 0, bytesToSend.Length);

            SendMessageEvent.Invoke(message); // Event

            ReceiveMessage();
        }

        private void ReceiveMessage()
        {
            if (ClientStream is null) return;

            byte[] bytesToRead = new byte[TcpClient.ReceiveBufferSize];
            int bytesRead = ClientStream.Read(bytesToRead, 0, TcpClient.ReceiveBufferSize);
            string ReceivedMessage = Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);

            GetMessageEvent.Invoke(ReceivedMessage); // Event
        }

        public void OpenSocket()
        {
            TcpClient = new TcpClient(IP, PORT);
            ClientStream = TcpClient.GetStream();
        }
        public void CloseSocket()
        {
            if (TcpClient is null) return;
            
            TcpClient.Close();
        }
    }
}
