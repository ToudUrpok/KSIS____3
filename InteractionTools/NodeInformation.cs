using System;
using System.Linq;
using System.Net;

namespace InteractionTools
{
    public class NodeInformation
    {
        public IPAddress NodeIP { get; }

        public NodeInformation()
        {
            NodeIP = GetNodeIP();
        }
        private IPAddress GetNodeIP()
        {
            IPAddress[] adresses = Dns.GetHostAddresses(Dns.GetHostName());
            IPAddress currentIPAdress = null;
            bool IsFound = false;
            foreach (var adress in adresses)
            {
                if (adress.GetAddressBytes().Length == 4 && !IsFound)
                {
                    currentIPAdress = adress;
                    IsFound = true;
                }    
            }
            return currentIPAdress;
            //return Dns.GetHostEntry(Dns.GetHostName()).AddressList.First();
        }
    }
}
