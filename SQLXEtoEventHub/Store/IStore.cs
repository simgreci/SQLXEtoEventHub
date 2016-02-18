using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLXEtoEventHub.Store
{
    public interface IStore
    {
        void Update(XEvent.XEPosition pos);
        XEvent.XEPosition Read();
    }
}
