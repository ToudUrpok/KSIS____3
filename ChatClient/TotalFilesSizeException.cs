using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient
{
    class TotalFilesSizeException : Exception
    {
        public TotalFilesSizeException(string message)
        : base(message)
        { }
    }
}
