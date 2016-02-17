using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace SQLXEtoEventHub
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder();
            sb.DataSource = "localhost";
            sb.IntegratedSecurity = true;
            SqlConnection conn = new SqlConnection(sb.ConnectionString);
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandTimeout = 0;
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = "SELECT type, data FROM sys.fn_MSxe_read_event_stream ('GetEvents', 0)";
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var b = reader[1] as byte[];
                var s = System.Text.Encoding.Unicode.GetString(b);
                Console.WriteLine(s);

            }
        }
    }
}
