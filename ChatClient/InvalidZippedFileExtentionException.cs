using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient
{
    class InvalidZippedFileExtentionException : Exception
    {
        public InvalidZippedFileExtentionException(string message)
        : base(message)
        { }
    }
}
