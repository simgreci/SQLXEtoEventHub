using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace SQLXEtoEventHub
{
    public class EventHubWriter
    {
        static string eventHubName = "{event hub name}";
        static string connectionString = "{send connection string}";

        protected EventHubClient client;

        public EventHubWriter()
        {
            client = EventHubClient.CreateFromConnectionString(connectionString, eventHubName);
        }

        public void Send(string text)
        {
            client.Send(new EventData(Encoding.UTF8.GetBytes(text)));
        }
    }
}
