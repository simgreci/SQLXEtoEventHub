using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLXEtoEventHub.XEvent
{
    public class XEPayload
    {

        public XEPayload(System.Collections.Hashtable ht, XEPosition pos)
        {
            this.HashTable = ht;
            this.Position = pos;
        }

        //public XEvent Event { get; set;  }
        public XEPosition Position { get; set; } 

        public System.Collections.Hashtable HashTable { get; set; }
    }
}
