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
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(EventConsumer));

        public string XELPath { get; protected set; }

        public SqlConnection DBConnection { get; private set; }

        public IStore Store { get; set; }

        public EventConsumer(SqlConnection connection, string XELPath, IStore Store)
        {
            this.DBConnection = connection;
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
            using (this.DBConnection)
            {
                DBConnection.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = this.DBConnection;
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
                    List<XEPayload> payloads = new List<XEPayload>();

                    while (reader.Read())
                    {
                        XEvent.XEvent e = new XEvent.XEvent();
                        XEPosition posInner = new XEPosition() {
                            LastFile = reader["file_name"].ToString(),
                            Offset = Convert.ToInt32(reader["file_offset"]) };

                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(reader["event_data"].ToString());

                        e.EventTime = DateTime.Parse(doc.FirstChild.Attributes["timestamp"].Value);

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

        public void CheckpointPosition(XEPosition pos)
        {
            Store.Update(pos);
        }
    }
}