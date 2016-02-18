using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLXEtoEventHubSp
{
    public class XESession
    {
        public string Name { get; private set; }
        public string FilePath { get; private set; }

        public XESession(string name, string filePath)
        {
            this.Name = name;
            this.FilePath = filePath;
        } 
    }
}
