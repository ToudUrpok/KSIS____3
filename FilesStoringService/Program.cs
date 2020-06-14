using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesStoringService
{
    class Program
    {
        static void Main(string[] args)
        {
            HTTPServer httpServer = new HTTPServer();
            httpServer.CreateListeningThread();
        }
    }
}
