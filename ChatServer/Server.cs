using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using InteractionTools;
using System.Net.Sockets;
using System.Net;

namespace ChatServer
{
    
    public class Connection
    {
        public int Id;
        public Socket socket;

        public Connection(Socket s)
        {
            socket = s;
            Id = -1;
        }
    }
    public class Server
    {
        public static int PortNumber = 8005;
        
        public NodeInformation nodeInformation;
        public Serializer serializer;

        public DialogInfo CommonDialog; 
        public Dictionary<int, Client> Clients;
        public static int MaxClientsAmount = 10;
        public int ConnectionsCount;
        public Dictionary<int, Connection> Connections;

        public Server()
        {
            nodeInformation = new NodeInformation();
            serializer = new Serializer();
            CommonDialog = new DialogInfo("Common dialog", 0);
            Clients = new Dictionary<int, Client>();
            ConnectionsCount = 0;
            Connections = new Dictionary<int, Connection>();
        }
        public void StartListen()
        {
            Thread TCP = new Thread(new ThreadStart(TCPhandler.StartWork));
            Thread UDP = new Thread(new ThreadStart(UDPhandler.StartWork));
            TCP.Start();
            UDP.Start();
        }

        public void HandleMessage(LANMessage message, int connectionID)
        {
            switch (message.messageType)
            {
                case MessageType.Identification:
                    if (IsNewClient(message.SenderID)) 
                    {
                        Clients[message.SenderID] = new Client(message.SenderName, connectionID);
                    }
                    else
                    {
                        Clients[message.SenderID].IsConnected = true;
                        Clients[message.SenderID].ConnectionID = connectionID;
                        Clients[message.SenderID].UpdateDialogsState();
                    }
                    Connections[connectionID].Id = message.SenderID;
                    Console.WriteLine(message.SenderName + "  joined");
                    DialogInfoMethods.AddMessage(CommonDialog.MessagesHistory,
                        ref CommonDialog.UnreadMessCount,
                        new ChatMessage(message.SenderID, message.SenderName, "  joined", DateTime.Now));
                    SendMessage(new LANMessage(MessageType.ClientHistory, 
                        DialogInfoMethods.DictionaryIntoList(Clients[message.SenderID].Dialogs)), Connections[connectionID].socket);
                    NotifyClients(message.SenderID);
                    break;
                case MessageType.CommonMess:
                    message.SenderName = Clients[message.SenderID].Name;
                    SendToAll(message);
                    DialogInfoMethods.AddMessage(CommonDialog.MessagesHistory,
                        ref CommonDialog.UnreadMessCount,
                        new ChatMessage(message.SenderID, message.SenderName, message.IP + "  " + message.content, DateTime.Now));
                    break;
                case MessageType.UDPRequest:
                    Console.WriteLine("request");
                    UDPhandler.SendResponse(IPAddress.Parse(message.IP), message.Port, serializer.Serialize(new LANMessage(MessageType.UDPResponse, nodeInformation.NodeIP.ToString(), PortNumber)));
                    break;
                case MessageType.ClientExit:
                    Console.WriteLine(Clients[message.SenderID].Name + "  exit");
                    Connections.Remove(connectionID);
                    Clients[message.SenderID].IsConnected = false;
                    DialogInfoMethods.AddMessage(CommonDialog.MessagesHistory,
                        ref CommonDialog.UnreadMessCount,
                        new ChatMessage(message.SenderID, message.SenderName, "exit", DateTime.Now));
                    UpdateDialogsState();
                    SendToAll(message);
                    break;
                case MessageType.PrivateMess:
                    if (Connections.ContainsKey(Clients[message.Port].ConnectionID))
                    {
                        message.SenderName = Clients[message.SenderID].Name;
                        int receiverId = message.Port;
                        message.SenderName = Clients[message.SenderID].Name;
                        SendMessage(message, Connections[Clients[receiverId].ConnectionID].socket);
                        DialogInfoMethods.AddMessage(Clients[message.SenderID].Dialogs[receiverId].MessagesHistory,
                            ref Clients[message.SenderID].Dialogs[receiverId].UnreadMessCount,
                            new ChatMessage(message.SenderID, "me", message.content, DateTime.Now));
                        DialogInfoMethods.AddMessage(Clients[receiverId].Dialogs[message.SenderID].MessagesHistory,
                            ref Clients[receiverId].Dialogs[message.SenderID].UnreadMessCount,
                            new ChatMessage(message.SenderID, message.SenderName, message.content, DateTime.Now));
                    }
                    break;
                case MessageType.DialogData:
                        SendMessage(new LANMessage(MessageType.DialogData, Clients[message.SenderID].Dialogs[message.Port]), Connections[connectionID].socket);
                    break;
            }
        }

        private void UpdateDialogsState()
        {
            foreach (Connection connection in Connections.Values)
            {
                Clients[connection.Id].UpdateDialogsState();
            }
        }

        public void HandleExit(int connectId)
        {
            if (Connections.ContainsKey(connectId))
            {
                Connections.Remove(connectId);
                foreach(KeyValuePair<int, Client> client in Clients)
                    if (client.Value.ConnectionID == connectId)
                    {
                        Clients[client.Key].IsConnected = false;
                        SendToAll(new LANMessage(MessageType.ClientExit, client.Key, client.Value.Name));
                        break;
                    }
                UpdateDialogsState();  
            }
        }
        public  void NotifyClients(int newClientId)
        {
            foreach (KeyValuePair<int, Connection> connection in Connections)
            {
                if (connection.Value.Id != newClientId)
                {
                    if (DialogExists(connection.Value.Id, newClientId))
                    {
                        Clients[connection.Value.Id].Dialogs[newClientId].IsActive = true;
                        SendMessage(new LANMessage(MessageType.ClientJoin, newClientId, Clients[connection.Value.Id].Dialogs[newClientId]), connection.Value.socket);
                    }
                    else 
                    {
                        Clients[connection.Value.Id].Dialogs.Add(newClientId, new DialogInfo(Clients[newClientId].Name, newClientId));
                        SendMessage(new LANMessage(MessageType.ClientJoin, newClientId, Clients[newClientId].Name), connection.Value.socket);
                    }
                }
            }
        }
        private bool DialogExists(int receiverId, int newClientId)
        {
            bool IsExists = false;
            foreach (KeyValuePair<int, DialogInfo> dialog in Clients[receiverId].Dialogs)
            {
                if (dialog.Key == newClientId)
                {
                    return IsExists = true;
                }
            }
            return IsExists;
        }

        public void SendToAll(LANMessage message)
        {
            if (Connections.Count > 0)
                foreach (Connection connection in Connections.Values)
                {
                    if (connection.Id != message.SenderID)
                        SendMessage(message, connection.socket);
                }
        }

        public void SendMessage(LANMessage message, Socket socket)
        {
            socket.Send(serializer.Serialize(message));
        }

        public bool IsNewClient(int id)
        {
            bool result = true;
            if (Clients.Count > 0)
                foreach (int key in Clients.Keys)
                {
                    if (key == id) {
                        return result = false;
                    }
                }
            return result;
        }
    }
}
