using System;
using SQLXEtoEventHub;
using SQLXEtoEventHub.XEvent;
using log4net;
using System.Data.SqlClient;
using SQLXEtoEventHub.Store;

namespace SQLXEtoEventHubCmd
{
    class Program
    {
        public const string EH_ENV = "EVENT_HUB_CONNECTION_STRING";
        public const string EH_NAME = "EVENT_HUB_NAME";
        public const string EH_PATH = "XE_PATH";
        public const string EH_SQL = "XE_DB";

        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        private static string _EventHubConnectionString;
        private static string _EventHubName;
        private static string _XEFilePath;
        private static string _XESQLConnString;

        static int Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("SQLXEtoEventHub.log4net.xml"));

            #region Read Environment Variables
            _EventHubConnectionString = Environment.GetEnvironmentVariable(EH_ENV);
            _EventHubName = Environment.GetEnvironmentVariable(EH_NAME);
            _XEFilePath = Environment.GetEnvironmentVariable(EH_PATH);
            _XESQLConnString = Environment.GetEnvironmentVariable(EH_SQL);

            if (string.IsNullOrEmpty(_EventHubConnectionString) | 
                string.IsNullOrEmpty(_EventHubName) | 
                string.IsNullOrEmpty(_XEFilePath) | 
                string.IsNullOrEmpty(_XESQLConnString))
            {
                log.Error("Must declare Environment Variables first!");
                return -100;
            }
            #endregion

            SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder("Server=localhost;Trusted_Connection=True;");
            DatabaseContext context = new DatabaseContext(sb.ConnectionString);

            RegistryStore rs = new RegistryStore(EH_NAME);
            EventConsumer ec = new EventConsumer(context, EH_PATH, rs);
            EventHubWriter ehw = new EventHubWriter(EH_NAME, EH_ENV);

            var events = ec.GetLastEvents();

            try
            {
                foreach (XEPayload pl in events)
                {
                    log.DebugFormat("Sending event {0:S}", pl.HashTable);
                    ehw.Send(pl.HashTable);
                    log.DebugFormat("Chechpointing position {0:S}", pl.Position);
                    ec.CheckpointPosition(pl.Position);
                }
            }
            catch (Exception exce)
            {
                log.ErrorFormat("{0:S}", exce.Message);
                return -234;
            }

            log.Info("Processing completed.");
            return 0;
        }
    }
}
