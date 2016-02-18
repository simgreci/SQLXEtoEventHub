using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using System.Xml.Linq;

namespace SQLXEtoEventHub
{
    public class EventConsumer
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(EventConsumer));

        private string _lastFile = String.Empty;
        public Int32 _offset = 0;

        public string XELPath { get; protected set; }

        public string ConnectionString { get; private set; }

        public XEPosition.RegistryStore RegistryStore { get; set; }

        public EventConsumer(string ConnectionString, string XELPath, XEPosition.RegistryStore RegistryStore)
        {
            this.ConnectionString = ConnectionString;
            this.XELPath = string.Concat(XELPath, "\\*");
            this.RegistryStore = RegistryStore;
        }

        public string LastFile
        {
            get
            {
                return _lastFile;
            }
        }

        public Int32 Offset
        {
            get
            {
                return _offset;
            }
        }

        public List<XEPayload> GetLastEvents()
        {
            XEPosition.XEPosition pos;
            #region Read from registry
            try
            {
                pos = RegistryStore.Read();
            }
            catch (Exception exce)
            {
                log.WarnFormat("Key missing? {0:S}", exce.Message);
                pos = new XEPosition.XEPosition();
            }
            #endregion

            #region Data retrieval
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "select * from sys.fn_xe_file_target_read_file(@path,null,@file,@offset) a";

                    var pathParam = new SqlParameter("path", SqlDbType.NVarChar, 4000);
                    pathParam.Value = XELPath;

                    SqlParameter fileParam;                
                    if (_lastFile.Equals(String.Empty))
                    {
                        fileParam = new SqlParameter("file", DBNull.Value);
                    }
                    else
                    {
                        fileParam = new SqlParameter("file", pos.LastFile);
                    }

                    SqlParameter offsetParam;
                    if (_offset.Equals(0))
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
                    List<XEPayload> payloads = new List<XEPayload>();

                    while (reader.Read())
                    {
                        _lastFile = reader["file_name"].ToString();
                        _offset = Convert.ToInt32(reader["file_offset"]);
                        XEvent e = new XEvent();
                        XEPosition.XEPosition posInner = new XEPosition.XEPosition() { LastFile = _lastFile, Offset = _offset };

                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(reader["event_data"].ToString());

                        e.TimeStamp = DateTime.Parse(doc.FirstChild.Attributes["timestamp"].Value);

                        foreach (XmlNode node in doc.SelectNodes("/event/data"))
                        {
                            switch (node.Attributes[0].Value)
                            {
                                case "error_number":
                                    e.ErrorNumber = Convert.ToInt32(node.LastChild.InnerText);
                                    break;
                                case "message":
                                    e.ErrorMessage = node.LastChild.InnerText;
                                    break;
                                case "severity":
                                    e.ErrorSeverity = Convert.ToInt16(node.LastChild.InnerText);
                                    break;
                            }
                        }

                        payloads.Add(new XEPayload() { Event = e, Position = posInner });
                    }
                    return payloads;
                }
            }
            #endregion
        }

        public void CheckpointPosition(XEPosition.XEPosition pos)
        {
            RegistryStore.Update(pos);
        }
    }
}