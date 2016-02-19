using Microsoft.SqlServer.Server;
using SQLXEtoEventHub;
using SQLXEtoEventHub.Store;
using System;

namespace SQLXEtoEventHubSp
{   
    public class StoredProcedure
    {
        [SqlProcedure()]
        public static void sp_send_xe_to_eventhub(string trace_name, string event_hub_connection, string event_hub_name)
        {
            if (string.IsNullOrEmpty(trace_name))
                throw new ArgumentNullException("@trace_name", "Parameter is required.");

            if(string.IsNullOrEmpty(event_hub_connection))
                throw new ArgumentNullException("@event_hub_connection", "Parameter is required.");

            if (string.IsNullOrEmpty(event_hub_name))
                throw new ArgumentNullException("@event_hub_name", "Parameter is required.");

            if (SqlContext.IsAvailable)
            {
                DatabaseContext context = new DatabaseContext("context connection = true");
                if (DBHelper.XESessionExist(context, trace_name))
                {
                    XESession session = DBHelper.GetSession(context, trace_name);
                    RegistryStore rs = new RegistryStore(trace_name);
                    EventConsumer c = new EventConsumer(context, session.FilePath, rs);
                }
                else
                    throw new Exception(String.Format("Session {0:S} is not valid or ",""));
            }
        }
    }
}
