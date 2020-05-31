using System;
using System.Collections.Generic;
using System.Text;
using InteractionTools;

namespace ChatServer
{
    public class Client
    {
        public bool IsConnected;
        public int ConnectionID;
        public string Name;
        const int CommonDialogID = 0;

        public Dictionary<int, DialogInfo> Dialogs;
        public Client(string name, int connectionId)
        {
            IsConnected = true;
            ConnectionID = connectionId;
            Name = name;
            Dialogs = new Dictionary<int, DialogInfo>
            {
                { CommonDialogID, Program.server.CommonDialog }
            };         
            AddActiveDialogs();
        }

        private void AddActiveDialogs()
        {
            foreach (KeyValuePair<int, Client> client in Program.server.Clients)
            {
                if (client.Value.IsConnected)
                {
                    Dialogs[client.Key] = new DialogInfo(client.Value.Name, client.Key);
                }
            }
        }

        public void UpdateDialogsState()
        {
            foreach (KeyValuePair<int, DialogInfo> dialog in Dialogs)
            {
                if (dialog.Key != CommonDialogID)
                    dialog.Value.IsActive = Program.server.Clients[dialog.Key].IsConnected;
            }
        }
    }
}
