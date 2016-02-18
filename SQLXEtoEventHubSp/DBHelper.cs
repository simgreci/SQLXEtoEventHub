using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Xml;

namespace SQLXEtoEventHubSp
{
    public class DBHelper
    {
        public static bool XESessionExist(SqlConnection conn, string sessionName)
        {
            bool exists = false;
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.Parameters.Add(BuildSqlParam("session_name", sessionName));
                cmd.CommandText = "select 1 from sys.dm_xe_sessions where name = @session_name";
                cmd.CommandType = System.Data.CommandType.Text;
                if (cmd.ExecuteScalar() != null)
                    exists = true;
            }
            return exists;
        }

        public static XESession GetSession(SqlConnection conn, string sessionName)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Parameters.Add(BuildSqlParam("session_name", sessionName));
                cmd.Connection = conn;
                cmd.CommandText = @"select s.address, s.name, t.target_data 
                                    from sys.dm_xe_sessions s
                                    inner
                                    join sys.dm_xe_session_targets t on s.address = t.event_session_address
                                    where s.name = @session_name
                                    and t.target_name = 'event_file'
                                    ";
                cmd.CommandType = System.Data.CommandType.Text;
                SqlDataReader reader = cmd.ExecuteReader();
            }
            return null;
        }

        private static string ExtractXESessionFilePath(XmlDocument doc)
        {
            return String.Empty;
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
