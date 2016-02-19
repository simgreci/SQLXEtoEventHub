using Microsoft.SqlServer.Server;
using SQLXEtoEventHub;
using SQLXEtoEventHub.Store;
using System.Data.SqlClient;

namespace SQLXEtoEventHubSp
{   
   
    public class StoredProcedure
    {
        [SqlProcedure()]
        public static void sp_send_xe_to_eventhub(string trace_name, string event_hub_connection, string event_hub_name)
        {
            DatabaseContext context = new DatabaseContext("Server=localhost;Trusted_Connection=True;");
            if(DBHelper.XESessionExist(context, trace_name))
            {
                XESession session = DBHelper.GetSession(context, trace_name);
                RegistryStore rs = new RegistryStore(trace_name);
                EventConsumer c = new EventConsumer(null, session.FilePath, rs);
            }
        }
    }
}
