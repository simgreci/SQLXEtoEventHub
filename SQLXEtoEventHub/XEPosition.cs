using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLXEtoEventHub
{
    public class XEPosition
    {
        public string LastFile { get; set; }
        public long Offset { get; set; }
    }
}
