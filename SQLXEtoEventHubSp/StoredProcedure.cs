using Microsoft.SqlServer.Server;
using SQLXEtoEventHub;

namespace SQLXEtoEventHubSp
{   
   
    public class StoredProcedure
    {
        [SqlProcedure()]
        public static void sp_send_xe_to_eventhub(string trace_name, string event_hub_connection, string event_hub_name)
        {
            if(DBHelper.XESessionExist(trace_name))
            {
            }
        }
    }
}
