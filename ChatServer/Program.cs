using System;

namespace ChatServer
{
    class Program
    {
        public static Server server = new Server();
        static void Main(string[] args)
        {
            server.StartListen();
        }
    }
}
