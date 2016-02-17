using System;
using log4net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using SQLXEtoEventHub.XEPosition;

namespace SQLXEtoEventHub
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("SQLXEtoEventHub.log4net.xml"));

            RegistryStore rs = new RegistryStore(System.Configuration.ConfigurationManager.AppSettings["EventHubName"]);
            EventConsumer ec = new EventConsumer(System.Configuration.ConfigurationManager.ConnectionStrings["SQLServerConnectionString"].ConnectionString);
            EventHubWriter ehw = new EventHubWriter(
                System.Configuration.ConfigurationManager.AppSettings["EventHubName"],
                System.Configuration.ConfigurationManager.AppSettings["EventHubConnectionString"]);

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

            Parallel.ForEach(events, (e) => { ehw.Send(e); });
        }
    }
}
