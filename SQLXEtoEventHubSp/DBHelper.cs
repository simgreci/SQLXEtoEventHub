using System;
using System.Data.SqlClient;
using System.Xml;
using System.IO;
using SQLXEtoEventHub;

namespace SQLXEtoEventHubSp
{
    public class DBHelper
    {
        public static bool XESessionExist(IDatabaseContext context, string sessionName)
        {
            bool exists = false;
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = context.Connection;
                cmd.Parameters.Add(BuildSqlParam("session_name", sessionName));
                cmd.CommandText = "select 1 from sys.dm_xe_sessions where name = @session_name";
                cmd.CommandType = System.Data.CommandType.Text;
                if (cmd.ExecuteScalar() != null)
                    exists = true;
            }
            return exists;
        }

        public static XESession GetSession(IDatabaseContext context, string sessionName)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Parameters.Add(BuildSqlParam("session_name", sessionName));
                cmd.Connection = context.Connection;
                cmd.CommandText = @"select s.address, s.name, t.target_data 
                            from sys.dm_xe_sessions s
                            inner
                            join sys.dm_xe_session_targets t on s.address = t.event_session_address
                            where s.name = @session_name
                            and t.target_name = 'event_file'
                            ";
                cmd.CommandType = System.Data.CommandType.Text;
                SqlDataReader reader = cmd.ExecuteReader();
                if (!reader.HasRows)
                    throw new Exception(String.Format("Target type of event_file is required for session {0:S}", sessionName));

                reader.Read();

                return new XESession(
                    reader["name"].ToString(),
                    ExtractXESessionFilePath(Convert.ToString(reader["target_data"]))
                    );
            }
        }

        private static string ExtractXESessionFilePath(string targetData)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(targetData);
            FileInfo f = new FileInfo(doc.SelectSingleNode("EventFileTarget/File").Attributes["name"].Value);
            return f.DirectoryName;
        }

        private static SqlParameter BuildSqlParam(string paramName, object value)
        {
            SqlParameter param = new SqlParameter();
            param.ParameterName = paramName;
            if (value.Equals(null))
                param.Value = DBNull.Value;
            else
                param.Value = value;
            return param;
        }
    }
}