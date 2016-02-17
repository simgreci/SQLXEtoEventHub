using System;
using log4net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using SQLXEtoEventHub.XEPosition;
using System.Configuration;
using SQLXEtoEventHub.Properties;

namespace SQLXEtoEventHub
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            Settings s = new Settings();
            log4net.Config.XmlConfigurator.Configure(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("SQLXEtoEventHub.log4net.xml"));

            RegistryStore rs = new RegistryStore(s.EventHubName);
            EventConsumer ec = new EventConsumer(s.SQLServerConnectionString);
            EventHubWriter ehw = new EventHubWriter(s.EventHubName,s.EventHubConnectionString);

            XEPosition.XEPosition pos = new XEPosition.XEPosition() { LastFile = string.Empty, Offset = 0 };

            try
            {
                pos = rs.Read();
            }
            catch(Exception exce)
            {
                Console.WriteLine("Key missing? " + exce.Message);
            }

            var events = ec.GetLastEvents(pos);

            var lastEvent = events.Last();

            //pos = new XEPosition.XEPosition { LastFile = lastEvent. } TODO!

            foreach(XEvent e in events)
            {
                ehw.Send(e);
            }
            //Parallel.ForEach(events, (e) => { ehw.Send(e); });
        }
    }
}
