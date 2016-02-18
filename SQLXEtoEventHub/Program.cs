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

            Settings s = new Settings();
            log4net.Config.XmlConfigurator.Configure(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("SQLXEtoEventHub.log4net.xml"));

            #region Read event hub connection string from env variables
            _EventHubConnectionString = Environment.GetEnvironmentVariable(EH_ENV);
            if (string.IsNullOrEmpty(_EventHubConnectionString))
            {
                log.ErrorFormat("Must declare {0:S} env variable first!", EH_ENV);
                return -100;
            }
            #endregion

            RegistryStore rs = new RegistryStore(s.EventHubName);
            EventConsumer ec = new EventConsumer(s.SQLServerConnectionString, s.XELPath, rs);
            EventHubWriter ehw = new EventHubWriter(s.EventHubName, _EventHubConnectionString);

            var events = ec.GetLastEvents();

            try
            {
                foreach (XEPayload pl in events)
                {
                    log.DebugFormat("Sending event {0:S}", pl.Event);
                    ehw.Send(pl.Event);

                    log.DebugFormat("Chechpointing position {0:S}", pl.Position);
                    ec.CheckpointPosition(pl.Position);
                }
            }
            catch(Exception exce)
            {
                log.ErrorFormat("{0:S}", exce.Message);
                return -234;
            }

            log.Info("Processing completed.");
            return 0;
        }
    }
}
