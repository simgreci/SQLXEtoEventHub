using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SQLXEtoEventHub
{
    public class EventHubWriter
    {
        public string EventHubName { get; set; }
        public string EventHubNameNamespace { get; set; }
        public string PolicyName { get; set; }

        public string PolicyKey { get; set; }

        public EventHubWriter(string EventHubName, string EventHubNameNamespace, string PolicyName, string PolicyKey)
        {
            this.EventHubName = EventHubName;
            this.EventHubNameNamespace = EventHubNameNamespace;
            this.PolicyKey = PolicyKey;
            this.PolicyName = PolicyName;
        }

        public static string Serialize(object o)
        {
            System.IO.StringWriter wr = new System.IO.StringWriter();
            JsonSerializer ser = new JsonSerializer();
            ser.Serialize(wr, o);
            wr.Flush();

            return wr.ToString();
        }

        public void Send(object o)
        {            
            Send(Serialize(o));
        }

        public void Send(string text)
        {
            EventHub.Publisher.PushToEventHub(
                EventHubNameNamespace,
                EventHubName,
                PolicyName,
                PolicyKey,
                TimeSpan.FromDays(7),
                text);
        }
    }
}
