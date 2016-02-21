using Microsoft.SqlServer.Server;
using SQLXEtoEventHub;
using SQLXEtoEventHub.Store;
using SQLXEtoEventHub.XEvent;
using System;
using System.Collections.Generic;

namespace SQLXEtoEventHubSp
{   
    public class StoredProcedure
    {
        private static void CheckNullParameter(string parameter, string parameter_name)
        {
            if (string.IsNullOrEmpty(parameter))
                throw new ArgumentNullException(parameter_name, "Parameter is required.");
        }

        [SqlProcedure()]
        public static void sp_send_xe_to_eventhub(string trace_name, string event_hub_name, string service_bus_namespace, string policy, string policy_key)
        {
            CheckNullParameter(trace_name, "@trace_name");
            CheckNullParameter(event_hub_name, "@event_hub_name");
            CheckNullParameter(service_bus_namespace, "@service_bus_namespace");
            CheckNullParameter(policy, "@policy");
            CheckNullParameter(policy_key, "@policy_key");

            if (SqlContext.IsAvailable)
            {
                DatabaseContext context = new DatabaseContext("context connection = true");
                using (context)
                {
                    if (DBHelper.XESessionExist(context, trace_name))
                    {
                        XESession session = DBHelper.GetSession(context, trace_name);
                        RegistryStore rs = new RegistryStore(trace_name);
                        EventConsumer c = new EventConsumer(context, session.FilePath, rs);
                        List<SQLXEtoEventHub.XEvent.XEPayload> payloads = c.GetLastEvents();

                        EventHubWriter writer = new EventHubWriter(event_hub_name, service_bus_namespace, policy, policy_key);
                        foreach (XEPayload p in payloads)
                        {
                            writer.Send(p);
                        }
                    }
                    else
                        throw new Exception(String.Format("Session {0:S} does not exists or is not running.", trace_name));
                }
            }
        }
    }
}
