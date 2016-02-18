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
        public const string EH_ENV = "EVENT_HUB_CONNECTION_STRING";
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        private static string _EventHubConnectionString;

        static int Main(string[] args)
        {
            #region Read event hub connection string from env variables
            _EventHubConnectionString = Environment.GetEnvironmentVariable(EH_ENV);
            if (string.IsNullOrEmpty(_EventHubConnectionString))
            {
                log.ErrorFormat("Must declare {0:S} env variable first!", EH_ENV);
                return -100;
            }
            #endregion

            Settings s = new Settings();
            log4net.Config.XmlConfigurator.Configure(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("SQLXEtoEventHub.log4net.xml"));

            RegistryStore rs = new RegistryStore(s.EventHubName);
            EventConsumer ec = new EventConsumer(s.SQLServerConnectionString);
            EventHubWriter ehw = new EventHubWriter(s.EventHubName, _EventHubConnectionString);

            XEPosition.XEPosition pos = new XEPosition.XEPosition() { LastFile = string.Empty, Offset = 0 };

            try
            {
                pos = rs.Read();
            }
            catch (Exception exce)
            {
                log.WarnFormat("Key missing? {0:S}", exce.Message);
            }

            var events = ec.GetLastEvents(pos);

            var lastEvent = events.Last();

            //pos = new XEPosition.XEPosition { LastFile = lastEvent. } TODO!

            foreach (XEvent e in events)
            {
                ehw.Send(e);
            }
            //Parallel.ForEach(events, (e) => { ehw.Send(e); });

            return 0;
        }
    }
}
