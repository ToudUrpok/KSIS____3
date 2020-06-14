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
using System.IO;
using System.Net.Http;

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

        public FilesService FilesService;
        private long TotalFilesSize;

        HTTPClientService HttpClientService;
        static public int HttpServerPort = 8007;
        static public string HttpServerDomain = "localhost";

        private List<int> AttachedFiles;
        private MatchingFiles MatchingFiles;

        public ClientForm()
        {
            InitializeComponent();
            this.FilesService = new FilesService();
            //FileIsLoading = false;
            TotalFilesSize = 0;
            AttachedFiles = new List<int>();
            MatchingFiles = new MatchingFiles();
            HttpClientService = new HTTPClientService();
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
                    if (message.AttachedFiles != null)
                    {
                        communityData.Dialogs[CommonDialogID].LoadedFiles.AddRange(message.AttachedFiles);
                    }
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
                    if (message.AttachedFiles != null)
                    {
                        communityData.Dialogs[message.SenderID].LoadedFiles.AddRange(message.AttachedFiles);
                    }
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
                    tbChatContent.Text += message.Time.ToString() + "  " + message.SenderName + ": " + message.Content + "\r\n";
                }
                if (AttachedFiles.Count > 0)
                {
                    AttachedFiles.Clear();
                    lbAttachedFiles.Items.Clear();
                    TotalFilesSize = 0;
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
                btShowLoadedFiles.Enabled = true;
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
            communityData = null;
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
            if ((tbMessageContent.Text.Length > 0) || (AttachedFiles.Count > 0))
            {
                LANMessage message = null;
                if (CurrentDialog == CommonDialogID)
                {
                    message = new LANMessage(MessageType.CommonMess, tbMessageContent.Text, client.ClientId, ((IPEndPoint)client.TCPsocket.LocalEndPoint).Address.ToString());
                }
                else
                {
                    message = new LANMessage(MessageType.PrivateMess, client.ClientId, MatchingDialogs[CurrentDialog].Id, ((IPEndPoint)client.TCPsocket.LocalEndPoint).Address.ToString(), tbMessageContent.Text);
                }
                if (AttachedFiles.Count > 0)
                {
                    communityData.Dialogs[MatchingDialogs[CurrentDialog].Id].LoadedFiles.AddRange(AttachedFiles);
                    message.AttachedFiles = new List<int>();
                    message.AttachedFiles.AddRange(AttachedFiles);
                    message.content += "\n   attached files:\n";
                    foreach (int fileID in message.AttachedFiles)
                    {
                        message.content += "  " + fileID.ToString() + "\n";
                    }
                }
                client.SendMessage(message);
                var newMessage = new ChatMessage(client.ClientId, "me", message.content, DateTime.Now);
                DialogInfoMethods.AddMessage(communityData.Dialogs[MatchingDialogs[CurrentDialog].Id].MessagesHistory,
                    ref communityData.Dialogs[MatchingDialogs[CurrentDialog].Id].UnreadMessCount,
                    newMessage);
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

        private void btAttachFile_Click(object sender, EventArgs e)
        {
            if (dlgLoadFile.ShowDialog() == DialogResult.OK)
            {
                btSendMessage.Enabled = false;
                string fileName = dlgLoadFile.FileName;

                if (!FilesService.IsValidFileExtension(fileName, FilesService.InvalidFilesExtensions))
                {
                    MessageBox.Show("Invalid file extension", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btSendMessage.Enabled = true;
                    return;
                }

                LoadFile(fileName);
            }
        }

        private async void LoadFile(string fileName)
        {
            byte[] buffer = null;
            try
            {
                buffer = await FilesService.ReadFile(fileName, TotalFilesSize);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btSendMessage.Enabled = true;
            }

            if (btSendMessage.Enabled)
            {
                return;
            }

            long length = buffer.Length;

            TotalFilesSize = TotalFilesSize + length;

            HttpContent httpContent = new ByteArrayContent(buffer);

            string requestUri = "http://" + HttpServerDomain + ":" + HttpServerPort.ToString() + "/" + fileName;
            try
            {
                int fileID = await HttpClientService.PostRequest(requestUri, httpContent);
                AddFileInListBox(fileID, Path.GetFileName(fileName), length);
                btSendMessage.Enabled = true;
            }
            catch (FileLoadException e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btSendMessage.Enabled = true;
                return;
            }
        }

        private void AddFileInListBox(int fileID, string fileName, long length)
        {
            MatchingFiles.AddFile(fileID, lbAttachedFiles.Items.Add(fileName + "  " + length.ToString()));
            AttachedFiles.Add(fileID);
        }

        private void RemoveFileFromListBox(int fileID)
        {
            AttachedFiles.Remove(fileID);
            lbAttachedFiles.Items.RemoveAt(MatchingFiles.IdMatchingDictionary[fileID]);
            List<string> rows = new List<string>();
            foreach (var row in lbAttachedFiles.Items)
            {
                    rows.Add((string)row);
            }
            lbAttachedFiles.Items.Clear();
            foreach(var row in rows)
            {
                lbAttachedFiles.Items.Add(row);
            }
            MatchingFiles.RemoveAllFiles();
            int i = 0;
            foreach(var file in AttachedFiles)
            {
                MatchingFiles.AddFile(file, i++);
            }
        }

        private void btDeleteFile_Click(object sender, EventArgs e)
        {
            if (lbAttachedFiles.SelectedIndex != -1)
            {
                int fileID = MatchingFiles.GetFileIDByMatchingID(lbAttachedFiles.SelectedIndex);
                RemoveFileFromListBox(fileID);
                DeleteFile(fileID);
            }
        }

        private async void DeleteFile(int fileID)
        {
            try
            {
                await HttpClientService.DeleteRequest("http://" + HttpServerDomain + ":" + HttpServerPort.ToString() + "/" + fileID.ToString());
            }
            catch (FileNotFoundException exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lbAttachedFiles_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btShowLoadedFiles_Click(object sender, EventArgs e)
        {
            if (communityData.Dialogs[MatchingDialogs[CurrentDialog].Id].LoadedFiles.Count > 0)
            {
                FilesViewingForm fvForm = new FilesViewingForm(HttpClientService, 
                    communityData.Dialogs[MatchingDialogs[CurrentDialog].Id].LoadedFiles);
                fvForm.Owner = this;
                fvForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("В данном диалоге нет вложенных файлов", "", MessageBoxButtons.OK);
            }
        }
    }
}
