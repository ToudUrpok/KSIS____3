using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
//using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace InteractionTools
{
    public class Serializer
    {
        /*private BinaryFormatter formatter;
        private MemoryStream buffer;

        public Serializer()
        {
            formatter = new BinaryFormatter();
        }

        public byte[] Serialize(LANMessage message)
        {
            buffer = new MemoryStream();
            formatter.Serialize(buffer, message);
            return buffer.GetBuffer();
        }

        public LANMessage Deserialize(byte[] bytes)
        {
            buffer = new MemoryStream(bytes);
            //buffer.Position = 0;
            buffer.Seek(0, SeekOrigin.Begin);
            return (LANMessage)formatter.Deserialize(buffer);
        }*/

        public byte[] Serialize(LANMessage message)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(LANMessage));
            MemoryStream messageStorage = new MemoryStream();
            serializer.Serialize(messageStorage, message);
            return messageStorage.GetBuffer();
        }

        public LANMessage Deserialize(byte[] data)
        {
            MemoryStream messageStorage = new MemoryStream();
            messageStorage.Write(data, 0, data.Length);  // 0 - смещение
            XmlSerializer serializer = new XmlSerializer(typeof(LANMessage));
            messageStorage.Position = 0;
            LANMessage message = (LANMessage)serializer.Deserialize(messageStorage);
            return message;
        }
    }
}
