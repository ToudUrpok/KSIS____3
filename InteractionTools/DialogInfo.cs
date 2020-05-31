using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using InteractionTools;

namespace InteractionTools
{
    [Serializable]
    public class DialogInfo : ICloneable
    {
        public bool IsActive;
        public string Name;
        public int Id;
        public List<ChatMessage> MessagesHistory;
        public int UnreadMessCount;

        public DialogInfo(string name, int id)
        {
            IsActive = true;
            Name = name;
            MessagesHistory = new List<ChatMessage>();
            UnreadMessCount = 0;
            Id = id;
        }

        public object Clone()
        {
            var dialog = (DialogInfo)this.MemberwiseClone();
            dialog.MessagesHistory = new List<ChatMessage>();
            foreach (ChatMessage message in this.MessagesHistory)
            {
                dialog.MessagesHistory.Add((ChatMessage)message.Clone());
            }
            return dialog;
        }

        public DialogInfo()
        { }
    }
}
