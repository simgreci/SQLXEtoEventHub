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
        private SqlConnection Connection;

        public DBHelper(SqlConnection conn)
        {
            this.Connection = conn;
        }
        public bool XESessionExist(string sessionName)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = this.Connection;
                cmd.CommandText = "select 1 from sys.dm_xe_sessions where name = @session_name";
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.ExecuteScalar();
                object o = cmd.ExecuteScalar();
                if ((bool)o)
                    return true;
                else
                    return false;
            }
        }

        public XESession GetSession(string sessionName)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Parameters.Add(BuildSqlParam("session_name", sessionName));
                cmd.Connection = this.Connection;
                cmd.CommandText = "SELECT 1 FROM ";
                cmd.CommandType = System.Data.CommandType.Text;
            }
            return new XESession("","");
        }

        private string ExtractXESessionFilePath(XmlDocument doc)
        {
            return String.Empty;
        }

        private SqlParameter BuildSqlParam(string paramName, object value)
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
