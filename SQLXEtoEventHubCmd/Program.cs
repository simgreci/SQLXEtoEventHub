using log4net;
using SQLXEtoEventHub;
using SQLXEtoEventHub.Store;
using SQLXEtoEventHub.XEvent;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SQLXEtoEventHubCmd
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        private static string _EventHubConnectionString;
        private static string _EventHubName;
        private static string _XEFilePath;
        private static string _XESQLConnString;

        static int Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("SQLXEtoEventHub.log4net.xml"));

            #region Read Environment Variables
            _EventHubConnectionString = Environment.GetEnvironmentVariable("EH_CONNECTION_STRING");
            _EventHubName = Environment.GetEnvironmentVariable("EH_NAME");
            _XEFilePath = Environment.GetEnvironmentVariable("EH_XESESSION_PATH");
            _XESQLConnString = Environment.GetEnvironmentVariable("EH_SQL_CONNECTION_STRING");

            if (string.IsNullOrEmpty(_EventHubConnectionString) ||
                string.IsNullOrEmpty(_EventHubName) ||
                string.IsNullOrEmpty(_XEFilePath) ||
                string.IsNullOrEmpty(_XESQLConnString))
            {
                log.Error("Must declare Environment Variables first!");
                return -100;
            }
            #endregion

            SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder("Server=localhost;Trusted_Connection=True;");
            IDatabaseContext context = new DatabaseContext(sb.ConnectionString);

            IStore rs = new RegistryStore(_EventHubName);
            EventConsumer ec = new EventConsumer(context, _XEFilePath, rs);
            EventHubWriter ehw = new EventHubWriter(_EventHubName, _EventHubConnectionString);

            var events = ec.GetLastEvents();

            Parallel.ForEach(events, (e) =>
            {
                ehw.Send(e);
                log.Info(e.Position.Offset.ToString());
            });

            //try
            //{
            //    foreach (XEPayload pl in events)
            //    {
            //        log.DebugFormat("Sending event {0:S}", pl.HashTable);
            //        ehw.Send(pl.HashTable);
            //        log.DebugFormat("Chechpointing position {0:S}", pl.Position);
            //        ec.CheckpointPosition(pl.Position);
            //    }
            //}
            //catch (Exception exce)
            //{
            //    log.ErrorFormat("{0:S}", exce.Message);
            //    return -234;
            //}

            log.Info("Processing completed.");
            return 0;
        }
    }
}
