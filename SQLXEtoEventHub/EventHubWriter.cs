using System;
using Newtonsoft.Json;

namespace SQLXEtoEventHub
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
                ServiceBusNamespace,
                EventHubName,
                PolicyName,
                PolicyKey,
                TimeSpan.FromDays(7),
                text);
        }
    }
}
