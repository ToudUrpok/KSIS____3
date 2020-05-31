using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace ChatServer
{
    static class TCPhandler
    {
        public static Socket TCPListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public static void StartWork() 
        {
            IPEndPoint serverEndPoint = new IPEndPoint(Program.server.nodeInformation.NodeIP, Server.PortNumber);
            try
            {
                TCPListener.Bind(serverEndPoint);
                TCPListener.Listen(Server.MaxClientsAmount);
                Console.WriteLine("Server is listning to TCP requests...");
                while (true)
                {
                    Socket connectionProvider = TCPListener.Accept();
                    Program.server.ConnectionsCount++;
                    Program.server.Connections.Add(Program.server.ConnectionsCount, new Connection(connectionProvider));
                    Thread receivingClientMessages = new Thread(new ParameterizedThreadStart(ReceiveMessages));
                    receivingClientMessages.Start(Program.server.ConnectionsCount); 
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static int BUFFER_SIZE = 1024; //in bytes
        static void ReceiveMessages(object connectionProviderId)
        {
            Socket connectionProvider = Program.server.Connections[(int)connectionProviderId].socket;
            while (connectionProvider.Connected)
            {
                byte[] buffer = new byte[BUFFER_SIZE];
                MemoryStream message = new MemoryStream();
                do
                {
                    try
                    {
                        int amount = connectionProvider.Receive(buffer);
                        message.Write(buffer, 0, amount);
                    }
                    catch (SocketException ex)
                    {
                        Console.WriteLine(ex.ErrorCode);
                        Console.WriteLine(ex.Message);
                        Program.server.HandleExit((int)connectionProviderId);
                    }
                    catch 
                    {

                    }
                }
                while (connectionProvider.Available != 0); //>0
                if (message.GetBuffer().Length > 0)
                    Program.server.HandleMessage(Program.server.serializer.Deserialize(message.GetBuffer()), (int)connectionProviderId);
            }
        }
    }
}
