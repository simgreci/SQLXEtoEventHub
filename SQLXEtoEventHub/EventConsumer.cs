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
        private string _lastFile = String.Empty;
        public Int32 _offset = 0;

        public EventConsumer()
        {
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

        public List<XEvent> GetLastEvents(string file, int offset)
        {
            SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder();
            sb.DataSource = "localhost";
            sb.IntegratedSecurity = true;
            SqlConnection conn = new SqlConnection(sb.ConnectionString);
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandTimeout = 0;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select * from sys.fn_xe_file_target_read_file('C:\\SqlServer\\output\\*',null,@file,@offset) a";

            SqlParameter fileParam;
            if (_lastFile.Equals(String.Empty))
            {
                fileParam = new SqlParameter("file", DBNull.Value);
            }
            else
            {
                fileParam = new SqlParameter("file", file);
            }

            SqlParameter offsetParam;
            if (_offset.Equals(0))
            {
                offsetParam = new SqlParameter("offset", DBNull.Value);
            }
            else
            {
                offsetParam = new SqlParameter("offset", offset);
            }


            cmd.Parameters.Add(fileParam);
            cmd.Parameters.Add(offsetParam);

            SqlDataReader reader = cmd.ExecuteReader();
            List<XEvent> events = new List<XEvent>();

            while (reader.Read())
            {
                _lastFile = reader["file_name"].ToString();
                _offset = Convert.ToInt32(reader["file_offset"]);
                XEvent e = new XEvent();

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

                events.Add(e);
            }
            return events;
        }
    }
}