using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Server;

namespace SQLXEtoEventHubSp
{   
   
    public class StoredProcedure
    {
        [SqlProcedure()]
        public static void sp_send_xe_to_eventhub()
        {

        }
    }
}
