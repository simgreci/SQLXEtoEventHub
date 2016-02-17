using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;

namespace SQLXEtoEventHub
{
    public class EventHubWriter
    {
        protected EventHubClient client;

        public string EventHubName { get; private set;  }
        public string ConnectionString { get; private set; }

        public EventHubWriter(string EventHubName, string ConnectionString)
        {
            this.EventHubName = EventHubName;
            this.ConnectionString = ConnectionString;

            client = EventHubClient.CreateFromConnectionString(ConnectionString, EventHubName);
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
            client.Send(new EventData(Encoding.UTF8.GetBytes(text)));
        }
    }
}
