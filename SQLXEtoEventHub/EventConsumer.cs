using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using System.Xml.Linq;
using SQLXEtoEventHub.XEvent;
using SQLXEtoEventHub.Store;

namespace SQLXEtoEventHub
{
    public class EventConsumer
    {
        public const string HT_SQL_SERVER_NAME = "SQLXEtoEventHub_SQLServer_Name";
        public const string HT_SQL_SERVER_VERSION = "SQLXEtoEventHub_SQLServer_Version";
        public const string HT_NAME = "SQLXEtoEventHub_name";
        public const string HT_EVENT_TIME = "SQLXEtoEventHub_event_time";

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(EventConsumer));

        public string XELPath { get; protected set; }

        public DatabaseContext DatabaseContext { get; private set; }

        public IStore Store { get; set; }

        public EventConsumer(DatabaseContext context, string XELPath, IStore Store)
        {
            this.DatabaseContext = context;
            this.XELPath = string.Concat(XELPath, "\\*");
            this.Store = Store;
        }

        public List<XEPayload> GetLastEvents()
        {
            XEPosition pos;
            #region Read from registry
            try
            {
                pos = Store.Read();
            }
            catch (Exception exce)
            {
                log.WarnFormat("Key missing? {0:S}", exce.Message);
                pos = new XEPosition();
            }
            #endregion

            #region Data retrieval
            using (this.DatabaseContext)
            {
                string sqlServerName;
                string sqlServerVersion;

                using (SqlCommand cmd = new SqlCommand("SELECT @@SERVERNAME, @@VERSION", this.DatabaseContext.Connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        sqlServerName = reader.GetString(0);
                        sqlServerVersion = reader.GetString(1);
                    }
                }

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = this.DatabaseContext.Connection;
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "select * from sys.fn_xe_file_target_read_file(@path,null,@file,@offset) a";

                    var pathParam = new SqlParameter("path", SqlDbType.NVarChar, 4000);
                    pathParam.Value = XELPath;

                    SqlParameter fileParam;
                    if (pos.LastFile.Equals(String.Empty))
                    {
                        fileParam = new SqlParameter("file", DBNull.Value);
                    }
                    else
                    {
                        fileParam = new SqlParameter("file", pos.LastFile);
                    }

                    SqlParameter offsetParam;
                    if (pos.Offset.Equals(0))
                    {
                        offsetParam = new SqlParameter("offset", DBNull.Value);
                    }
                    else
                    {
                        offsetParam = new SqlParameter("offset", pos.Offset);
                    }

                    cmd.Parameters.Add(pathParam);
                    cmd.Parameters.Add(fileParam);
                    cmd.Parameters.Add(offsetParam);

                    SqlDataReader reader = cmd.ExecuteReader();
                    var payloads = ParsePayloads(reader);

                    System.Threading.Tasks.Parallel.ForEach(payloads, (pl) =>
                    {
                        pl.HashTable[HT_SQL_SERVER_NAME] = sqlServerName;
                        pl.HashTable[HT_SQL_SERVER_VERSION] = sqlServerVersion;
                    });

                    return payloads;
                }
            }
            #endregion
        }

        protected static List<XEPayload> ParsePayloads(SqlDataReader reader)
        {
            List<XEPayload> payloads = new List<XEPayload>();

            while (reader.Read())
            {
                string eventName = reader.GetString(2);

                XEPosition posInner = new XEPosition()
                {
                    LastFile = reader.GetString(4),
                    Offset = reader.GetInt64(5)
                };

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(reader["event_data"].ToString());
                DateTime eventTime = DateTime.Parse(doc.FirstChild.Attributes["timestamp"].Value);

                System.Collections.Hashtable ht = new System.Collections.Hashtable();

                foreach (XmlNode node in doc.SelectNodes("/event/data"))
                {
                    string name = node.Attributes["name"].Value;
                    string value = node.SelectSingleNode("value").InnerText;

                    AddTyped(ht, name, value);
                }

                foreach (XmlNode node in doc.SelectNodes("/event/action"))
                {
                    string name = string.Join("_", node.Attributes["package"].Value, node.Attributes["name"].Value);
                    string value = node.SelectSingleNode("value").InnerText;

                    AddTyped(ht, name, value);
                }

                ht[HT_NAME] = eventName;
                ht[HT_EVENT_TIME] = eventTime;

                payloads.Add(new XEPayload(ht, posInner));
            }

            return payloads;
        }

        protected static void AddTyped(System.Collections.Hashtable ht, string name, string value)
        {
            long lVal;
            if (long.TryParse(value, out lVal))
            {
                ht[name] = lVal;
            }
            else
            {
                ht[name] = value;
            }
        }

        public void CheckpointPosition(XEPosition pos)
        {
            Store.Update(pos);
        }
    }
}