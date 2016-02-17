using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLXEtoEventHub
{
    public class XEvent
    {
        public XEvent()
        {
        }

        public string ErrorMessage { get; set; }
        public Int32 ErrorNumber { get; set; }
        public Int16 ErrorSeverity { get; set; }
        public string UserName { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}