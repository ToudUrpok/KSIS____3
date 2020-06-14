using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesStoringService
{
    public class FileInformation
    {
        public string Name;
        public string Size;

        public FileInformation(string name, string size)
        {
            Name = name;
            Size = size;
        }
    }
}
