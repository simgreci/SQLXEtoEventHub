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

        public override string ToString()
        {
            return string.Format("{0:S}[ErrorMessage=\"{1:S}\", ErrorNumber={2:N0}, ErrorSeverity={3:N0}, UserName=\"{4:S}\", TimeStamp={5:S}]",
                GetType().Name,
                ErrorMessage,
                ErrorNumber,
                ErrorSeverity,
                UserName,
                TimeStamp.ToString()
                );
        }
    }
}