using System;
using System.Collections.Generic;
using System.Text;

namespace InteractionTools
{
    [Serializable]
    public class ChatMessage : ICloneable
    {
        public int SenderId;
        public string SenderName;
        public string Content;
        public DateTime Time;

        public ChatMessage(int senderId,string name, string content, DateTime time)
        {
            SenderId = senderId;
            SenderName = name;
            Content = content;
            Time = time;
        }
        public object Clone()
        {
            var message = (ChatMessage)this.MemberwiseClone();
            return message;
        }

        public ChatMessage()
        {

        }
    }
}
