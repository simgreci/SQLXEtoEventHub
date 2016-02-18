using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace SQLXEtoEventHubSp
{
    public class DBHelper
    {
        SqlConnection Connection;

        public DBHelper(SqlConnection conn)
        {
            this.Connection = conn;
        }
        public static bool TraceExists(string traceName)
        {
            return true;
        }

        public static XESession GetTraceDetails(string traceName)
        {

        }
    }
}
