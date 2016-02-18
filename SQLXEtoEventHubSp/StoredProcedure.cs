using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Server;
using SQLXEtoEventHub;

namespace SQLXEtoEventHubSp
{   
   
    public class StoredProcedure
    {
        [SqlProcedure()]
        public static void sp_send_xe_to_eventhub(string trace_name, string event_hub_connection, string event_hub_name)
        {
            DBHelper helper = new DBHelper(new System.Data.SqlClient.SqlConnection("context"));
            if(helper.XESessionExists(trace_name))
            {

            }
        }
    }
}
