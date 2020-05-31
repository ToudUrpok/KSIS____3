using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InteractionTools;
using System.Net;

namespace ChatClient
{
    public struct ClientDialog
    {
        public int Id;
        public bool IsDownloaded;

        public ClientDialog(int id)
        {
            Id = id;
            IsDownloaded = (id == 0);
        }
    }
    public partial class ClientForm : Form
    {
        Client client;
        CommunityData communityData;
        Dictionary<int, ClientDialog> MatchingDialogs;
        const int CommonDialogID = 0;
        int CurrentDialog;
        public ClientForm()
        {
            InitializeComponent();
        }

        public void HandleMess(LANMessage message)
        {
            switch (message.messageType)
            {
                case MessageType.ClientHistory:
                    {
                        Action action = delegate
                        {
                            communityData = new CommunityData(DialogInfoMethods.ListIntoDictionary(message.Dialogs));
                            CurrentDialog = CommonDialogID;
                            MatchingDialogs = new Dictionary<int, ClientDialog>();
                            UpdateParticipants();
                            if (CommonDialogID == CurrentDialog)
                                UpdateContent();
                            lbCurrentDialog.Text = communityData.Dialogs[CommonDialogID].Name;
                        };
                        if (InvokeRequired)
                            Invoke(action);
                        else
                            action();
                    }
                    break;
                case MessageType.CommonMess:
                    DialogInfoMethods.AddMessage(communityData.Dialogs[CommonDialogID].MessagesHistory, 
                        ref communityData.Dialogs[CommonDialogID].UnreadMessCount, 
                        new ChatMessage(message.SenderID, message.SenderName, message.IP + "  " + message.content, DateTime.Now));
                    if (CommonDialogID == CurrentDialog)
                    {
                        UpdateContent();
                        communityData.Dialogs[CommonDialogID].UnreadMessCount--;
                    }
                    else
                    {
                        UpdateParticipants();
                    }
                        break;
                case MessageType.ClientJoin:
                    {
                        Action action = delegate
                        {
                            DialogInfoMethods.AddMessage(communityData.Dialogs[CommonDialogID].MessagesHistory,
                                ref communityData.Dialogs[CommonDialogID].UnreadMessCount, 
                                new ChatMessage(message.SenderID, message.SenderName, "  joined", DateTime.Now));
                            if (DialogExists(message.SenderID))
                            {
                                communityData.Dialogs[message.SenderID].IsActive = true;
                            }
                            else
                            {
                                communityData.Dialogs[message.SenderID] = new DialogInfo(message.SenderName, message.SenderID);
                            }

                            if (CommonDialogID == CurrentDialog)
                            {
                                UpdateContent();
                                communityData.Dialogs[CommonDialogID].UnreadMessCount--;
                            }
                            UpdateParticipants();
                        };
                        if (InvokeRequired)
                            Invoke(action);
                        else
                            action();
                    }
                    break;
                case MessageType.ClientExit:
                    communityData.Dialogs[message.SenderID].IsActive = false;
                    DialogInfoMethods.AddMessage(communityData.Dialogs[CommonDialogID].MessagesHistory,
                        ref communityData.Dialogs[CommonDialogID].UnreadMessCount,
                        new ChatMessage(message.SenderID, message.SenderName, "  exit", DateTime.Now));
                    if (CommonDialogID == CurrentDialog)
                    {
                        UpdateContent();
                        communityData.Dialogs[CommonDialogID].UnreadMessCount--;
                    }
                    UpdateParticipants();
                    break;
                case MessageType.PrivateMess:
                    DialogInfoMethods.AddMessage(communityData.Dialogs[message.SenderID].MessagesHistory,
                        ref communityData.Dialogs[message.SenderID].UnreadMessCount,
                        new ChatMessage(message.SenderID, message.SenderName, message.IP + "  " + message.content, DateTime.Now));
                    if (message.SenderID == CurrentDialog)
                    {
                        UpdateContent();
                        communityData.Dialogs[message.SenderID].UnreadMessCount--;
                    }
                    else
                    {
                        UpdateParticipants();
                    }
                    break;
                case MessageType.DialogData:
                    DialogInfo receivedDialog = message.Dialogs.Last();
                    communityData.Dialogs[receivedDialog.Id] = receivedDialog;
                    UpdateSelectedDialog();
                    break;
            }
        }

        private void UpdateParticipants()
        {
            Action action = delegate
            {
                lbParticipants.Items.Clear();
                int index = 0;
                MatchingDialogs.Clear();
                foreach (DialogInfo dialog in communityData.Dialogs.Values)
                {
                    MatchingDialogs.Add(index, new ClientDialog(dialog.Id));
                    lbParticipants.Items.Insert(index, ((dialog.IsActive) ?
                        "[on]" : "[off]") + "  " + dialog.Name + "  " + ((dialog.UnreadMessCount != 0) ?
                        dialog.UnreadMessCount.ToString() : ""));
                    index++;
                }
            };
            if (InvokeRequired)
                Invoke(action);
            else
                action();
        }

        private void UpdateContent()
        {
            Action action = delegate
            {
                tbChatContent.Clear();
                foreach (ChatMessage message in communityData.Dialogs[MatchingDialogs[CurrentDialog].Id].MessagesHistory)
                {
                    tbChatContent.Text += message.SenderName + ": " + message.Content + "  " + message.Time.ToString() + "\r\n";
                }
            };
            if (InvokeRequired)
                Invoke(action);
            else
                action();
        }

        private bool DialogExists(int newClientId)
        {
            bool IsExists = false;
            foreach (KeyValuePair<int, DialogInfo> dialog in communityData.Dialogs)
            {
                if (dialog.Key == newClientId)
                {
                    return IsExists = true;
                }
            }
            return IsExists;
        }

        private void btConnect_Click(object sender, EventArgs e)
        {
            client = new Client();
            client.MessageReceieved += HandleMess;
            client.SendUDPRequest();
            if (client.IsConnected)
            {
                cbIsConnected.Checked = true;
                tbIPAdress.Text = ((IPEndPoint)client.TCPsocket.RemoteEndPoint).Address.ToString();
                tbName.ReadOnly = tbPassword.ReadOnly = true;
                btConnect.Enabled = false;
                btDisconnect.Enabled = true;
                client.JoinChat(tbName.Text, tbPassword.Text.GetHashCode());
            }
        }

        private void btDisconnect_Click(object sender, EventArgs e)
        {
            client?.SendMessage(new LANMessage(MessageType.ClientExit, client.ClientId, tbName.Text));
            client?.Disconnect();
            client = null;
            btDisconnect.Enabled = false;
            btSendMessage.Enabled = false;
            foreach (DialogInfo dialog in communityData.Dialogs.Values)
            {
                dialog.IsActive = false;
            }
            UpdateParticipants();
            //communityData = null;
            btConnect.Enabled = true;
            cbIsConnected.Checked = false;
            tbName.ReadOnly = false;
            tbPassword.ReadOnly = false;
        }

        private void ClientForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            client?.SendMessage(new LANMessage(MessageType.ClientExit, client.ClientId, tbName.Text));
            client?.Disconnect();
            client = null;
            communityData = null;
        }

        private void btSendMessage_Click(object sender, EventArgs e)
        {
            if ((tbMessageContent.Text.Length > 0))
            {
                if (CurrentDialog == CommonDialogID)
                {
                    client.SendMessage(new LANMessage(MessageType.CommonMess, tbMessageContent.Text, client.ClientId, ((IPEndPoint)client.TCPsocket.LocalEndPoint).Address.ToString()));
                }
                else
                {
                    client.SendMessage(new LANMessage(MessageType.PrivateMess, client.ClientId, MatchingDialogs[CurrentDialog].Id, ((IPEndPoint)client.TCPsocket.LocalEndPoint).Address.ToString(), tbMessageContent.Text));
                }
                DialogInfoMethods.AddMessage(communityData.Dialogs[MatchingDialogs[CurrentDialog].Id].MessagesHistory,
                    ref communityData.Dialogs[MatchingDialogs[CurrentDialog].Id].UnreadMessCount,
                    new ChatMessage(client.ClientId, "me", tbMessageContent.Text, DateTime.Now));
                UpdateContent();
                communityData.Dialogs[MatchingDialogs[CurrentDialog].Id].UnreadMessCount--;
                tbMessageContent.Clear();
            }
        }

        private void lbParticipants_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbParticipants.SelectedIndex == -1)
                CurrentDialog = CommonDialogID;
            if (CurrentDialog != lbParticipants.SelectedIndex)
            {
                CurrentDialog = lbParticipants.SelectedIndex;
                lbCurrentDialog.Text = communityData.Dialogs[MatchingDialogs[CurrentDialog].Id].Name;
                if (!MatchingDialogs[CurrentDialog].IsDownloaded)
                {
                    client.SendMessage(new LANMessage(MessageType.DialogData, client.ClientId, MatchingDialogs[CurrentDialog].Id));
                }
                else
                {
                    UpdateSelectedDialog();
                }
            }
        }

        private void UpdateSelectedDialog()
        {
            UpdateContent();
            btSendMessage.Enabled = communityData.Dialogs[MatchingDialogs[CurrentDialog].Id].IsActive;
            communityData.Dialogs[MatchingDialogs[CurrentDialog].Id].UnreadMessCount = 0;
            UpdateParticipants();
        }
    }
}
