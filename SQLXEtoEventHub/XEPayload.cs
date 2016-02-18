using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLXEtoEventHub
{
    public class XEPayload
    {
        public XEvent Event { get; set;  }
        public XEPosition.XEPosition Position { get; set; } 
    }
}
