using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using InteractionTools;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Linq.Expressions;

namespace ChatClient
{
    public class Client
    {
        public Socket TCPsocket;
        readonly Serializer messageSerializer;
        public bool IsConnected = false;
        readonly NodeInformation nodeInfo;
        public int ClientId;
        const int ServerPort = 8005;
        const int LocalPort = 5555;

        public delegate void HandleMessage(LANMessage message);
        public event HandleMessage MessageReceieved;
        public Thread receiveMess;

        public Client()
        {
            messageSerializer = new Serializer();
            nodeInfo = new NodeInformation();
        }

        public void SendUDPRequest()
        {
            Socket UDPsocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint serverEndPoint = new IPEndPoint(nodeInfo.NodeIP, LocalPort);
            UDPsocket.Bind(serverEndPoint);

            Socket sendUDPRequest = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
            {
                EnableBroadcast = true
            };
            sendUDPRequest.SendTo(messageSerializer.Serialize(new LANMessage(MessageType.UDPRequest, nodeInfo.NodeIP.ToString(), LocalPort)),
                new IPEndPoint(IPAddress.Broadcast, ServerPort));

            ReceiveUDPResponse(UDPsocket);
        }

        public void ReceiveUDPResponse(Socket socket)
        {
            byte[] data = new byte[1024];
            EndPoint endPoint = new IPEndPoint(IPAddress.Any, ServerPort);
            while (true)
            {
                int count = socket.ReceiveFrom(data, ref endPoint);
                Array.Resize(ref data, count);
                LANMessage message = messageSerializer.Deserialize(data);
                if (message.messageType == MessageType.UDPResponse)
                {
                    socket.Close();
                    SetConnection(new IPEndPoint(IPAddress.Parse(message.IP), message.Port));
                    return;
                }
            }   
        }
        public void Disconnect()
        {
            TCPsocket.Close();
            IsConnected = false;
            receiveMess.Abort();
        }

        public void JoinChat(string name, int id)
        {
            ClientId = id;
            receiveMess = new Thread(ReceiveMessages);
            receiveMess.Start();
            SendMessage(new LANMessage(MessageType.Identification, name, id, ((IPEndPoint)TCPsocket.LocalEndPoint).Address.ToString()));
        }

        static int BUFFER_SIZE = 1024; //in bytes
        public void ReceiveMessages()
        {
            while (TCPsocket.Connected)
            {
                byte[] buffer = new byte[BUFFER_SIZE];
                MemoryStream message = new MemoryStream();
                do
                {
                    try
                    {
                        int amount = TCPsocket.Receive(buffer);
                        message.Write(buffer, 0, amount);
                    }
                    catch (SocketException ex)
                    {
                        Console.WriteLine(ex.ErrorCode);
                        Console.WriteLine(ex.Message);
                    }
                    catch
                    {

                    }
                }
                while (TCPsocket.Available != 0); //>0
                if (message.GetBuffer().Length > 0)
                    MessageReceieved(messageSerializer.Deserialize(message.GetBuffer()));
            }
        }

        public void SendMessage(LANMessage message)
        {
            try
            {
                TCPsocket.Send(messageSerializer.Serialize(message));
            }
            catch(Exception ex)
            {
                Disconnect();
                MessageBox.Show(ex.Message);
            }
        }

        public void SetConnection(IPEndPoint IPEnd)
        {
            try
            {
                TCPsocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                TCPsocket.Connect(IPEnd);
                IsConnected = true;
            }
            catch (Exception ex)
            {
                TCPsocket.Close();
                MessageBox.Show(ex.Message);
            }
        }
    }
}
