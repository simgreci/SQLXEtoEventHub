using System;
using xetohub.core.eventhub;
using xetohub.core.xevent;

namespace xetohub.core
{
    public class EventHubWriter
    {
        public string EventHubName { get; set; }
        public string ServiceBusNamespace { get; set; }
        public string PolicyName { get; set; }

        public string PolicyKey { get; set; }

        public EventHubWriter(string EventHubName, string ServiceBusNamespace, string PolicyName, string PolicyKey)
        {
            this.EventHubName = EventHubName;
            this.ServiceBusNamespace = ServiceBusNamespace;
            this.PolicyKey = PolicyKey;
            this.PolicyName = PolicyName;
        }

        public static string Serialize(System.Collections.Generic.Dictionary<string, object> ht)
        {
            System.IO.StringWriter wr = new System.IO.StringWriter();
            wr.Write("{");

            //this should guarantee english decimal separators. If not, try en-us.
            //System.Security.SecurityException: Request for the permission of type 'System.Security.Permissions.SecurityPermission, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089' failed.
            //System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            bool fFirst = true;

            foreach (var kvp in ht)
            {
                if (!fFirst)
                {
                    wr.WriteLine(",");
                }
                else
                {
                    wr.WriteLine();
                    fFirst = false;
                }
                wr.Write(string.Format("\t\"{0:S}\": ", kvp.Key));

                if (kvp.Value is double)
                    wr.Write(((double)kvp.Value).ToString());
                else if (kvp.Value is int)
                    wr.Write(((int)kvp.Value).ToString());
                else if (kvp.Value is long)
                    wr.Write(((long)kvp.Value).ToString());
                else if (kvp.Value is DateTime)
                    wr.Write(UnixTicks(((DateTime)kvp.Value)).ToString());
                else
                    wr.Write(string.Format("\"{0:S}\"", kvp.Value.ToString()));
            }

            wr.WriteLine();
            wr.WriteLine("}");
            wr.Flush();

            return wr.ToString();
        }

        public void Send(XEPayload payload)
        {
            Send(Serialize(payload.Dictionary));
        }

        public void Send(string text)
        {
            Publisher.PushToEventHub(
                ServiceBusNamespace,
                EventHubName,
                PolicyName,
                PolicyKey,
                TimeSpan.FromDays(7),
                text);
        }

        // returns the number of milliseconds since Jan 1, 1970 (useful for converting C# dates to JS dates)
        public static double UnixTicks(DateTime dt)
        {
            DateTime d1 = new DateTime(1970, 1, 1);
            DateTime d2 = dt.ToUniversalTime();
            TimeSpan ts = new TimeSpan(d2.Ticks - d1.Ticks);
            return ts.TotalMilliseconds;
        }
    }
}