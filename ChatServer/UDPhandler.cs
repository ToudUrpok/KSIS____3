using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ChatServer
{
    static class UDPhandler
    {
        public static Socket socketListener = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        public static void StartWork()
        {
            IPEndPoint serverEndPoint = new IPEndPoint(Program.server.nodeInformation.NodeIP, Server.PortNumber);
            socketListener.Bind(serverEndPoint);
            Console.WriteLine("UDP");
            byte[] data = new byte[1024];
            int count;
            EndPoint endPoint = new IPEndPoint(IPAddress.Any, 5555);
            while (true)
            {
                count = socketListener.ReceiveFrom(data, ref endPoint);
                Array.Resize(ref data, count);
                Program.server.HandleMessage(Program.server.serializer.Deserialize(data), 0);
            }
        }

        public static void SendResponse(IPAddress IP, int Port, byte[] message)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint receiverEndPoint = new IPEndPoint(IP, Port);
            socket.SendTo(message, receiverEndPoint);
            Console.WriteLine("response");
        }
    }
}
