using Microsoft.SqlServer.Server;
using xetohub.core;
using xetohub.core.store;
using xetohub.core.xevent;
using System;
using System.Collections.Generic;

namespace xetohub.sql
{
    public class storedprocedures
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

            List<XEPayload> payloads = null;

            SqlContext.Pipe.Send("Opening context connection...");

            if (SqlContext.IsAvailable)
            {
                using (DatabaseContext context = new DatabaseContext("context connection = true"))
                {
                    using (context)
                    {
                        if (DBHelper.XESessionExist(context, trace_name))
                        {
                            SqlContext.Pipe.Send("Reading XE session info...");
                            XESession session = DBHelper.GetSession(context, trace_name);

                            SqlContext.Pipe.Send("Reading last position...");
                            RegistryStore rs = new RegistryStore(trace_name);

                            SqlContext.Pipe.Send(string.Format("last position is {0:S}", rs.ToString()));
                            EventConsumer c = new EventConsumer(context, session.FilePath, rs);

                            SqlContext.Pipe.Send("reading events...");
                            payloads = c.GetLastEvents();
                            SqlContext.Pipe.Send(string.Format("{0:N0} events read", payloads.Count));
                        }
                        else
                            throw new Exception(String.Format("Session {0:S} does not exists or is not running.", trace_name));
                    }
                }
            }

            if (payloads != null)
            {
                EventHubWriter writer = new EventHubWriter(event_hub_name, service_bus_namespace, policy, policy_key);
                foreach (XEPayload p in payloads)
                {
                    SqlContext.Pipe.Send("Sending one event to EH");
                    writer.Send(p);
                }
            }
        }
    }
}

