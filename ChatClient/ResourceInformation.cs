using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient
{
    public class ResourceInformation
    {
        public string Name;
        public string Size;

        public ResourceInformation(string name, string size)
        {
            Name = name;
            Size = size;
        }
    }
}
