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
            if(DBHelper.XESessionExist(trace_name))
            {
                SqlConnection conn = new SqlConnection("Server=localhost;Trusted_Connection=True;");
                XESession session = DBHelper.GetSession(trace_name);
                RegistryStore rs = new RegistryStore(trace_name);
                EventConsumer c = new EventConsumer(conn, session.FilePath, rs);
            }
        }
    }
}
