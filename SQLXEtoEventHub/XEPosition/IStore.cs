using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLXEtoEventHub.XEPosition
{
    public interface IStore
    {
        void Update(XEPosition pos);
        XEPosition Read();
    }
}
