using log4net;
using SQLXEtoEventHub;
using SQLXEtoEventHub.Store;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SQLXEtoEventHubCmd
{
    class Program
    {
        public const string EH_NAMESPACE = "EH_NAMESPACE";
        public const string EH_NAME = "EH_NAME";
        public const string EH_POLICY = "EH_POLICY";
        public const string EH_POLICY_KEY = "EH_POLICY_KEY";
        public const string EH_PATH = "XE_PATH";
        public const string EH_SQL = "XE_DB";

        private static readonly ILog log = LogManager.GetLogger(typeof(Program));
        private static string _ServiceBusNamespace;
        private static string _EventHubName;
        private static string _EventHubPolicy;
        private static string _EventHubPolicyKey;
        private static string _XEFilePath;
        private static string _XESQLConnString;

        private static string GetEnvOrError(string variable)
        {
            var s = Environment.GetEnvironmentVariable(variable);
            if (string.IsNullOrEmpty(s))
                throw new EnvironmentVariableNotSetException(variable);

            return s;
        }

        static int Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("SQLXEtoEventHub.log4net.xml"));

            #region Read Environment Variables

            _EventHubName = Environment.GetEnvironmentVariable("EH_NAME");
            _XEFilePath = Environment.GetEnvironmentVariable("EH_XESESSION_PATH");
            _XESQLConnString = Environment.GetEnvironmentVariable("EH_SQL_CONNECTION_STRING");

            if (string.IsNullOrEmpty(_EventHubName) ||
                string.IsNullOrEmpty(_XEFilePath) ||
                string.IsNullOrEmpty(_XESQLConnString))
            {
                log.Error("Must declare Environment Variables first!");
                return -100;
            }

            _ServiceBusNamespace = GetEnvOrError(EH_NAMESPACE);
            _EventHubName = GetEnvOrError(EH_NAME);
            _EventHubPolicy = GetEnvOrError(EH_POLICY);
            _EventHubPolicyKey = GetEnvOrError(EH_POLICY_KEY);

            _XEFilePath = GetEnvOrError(EH_PATH);
            _XESQLConnString = GetEnvOrError(EH_SQL);

            #endregion

            SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder("Server=localhost;Trusted_Connection=True;");
            IDatabaseContext context = new DatabaseContext(sb.ConnectionString);

            RegistryStore rs = new RegistryStore(_EventHubName);
            EventConsumer ec = new EventConsumer(context, _XEFilePath, rs);
            EventHubWriter ehw = new EventHubWriter(_EventHubName, _ServiceBusNamespace, _EventHubPolicy, _EventHubPolicyKey);

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
