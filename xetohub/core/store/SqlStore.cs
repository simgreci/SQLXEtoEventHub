using System;
using System.Data;
using System.Data.SqlClient;
using xetohub.core.xevent;

namespace xetohub.core.store
{
    public class SqlStore : IStore
    {
        private IDatabaseContext _sqlContext;
        private string _sessionName;
        public SqlStore (IDatabaseContext context, string sessionName)
        {
            _sqlContext = context;
            _sessionName = sessionName;
        }
        XEPosition IStore.Read()
        {
            using (SqlConnection conn = _sqlContext.Connection)
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "dbo.xetohub_read_offset";
                    SqlParameter param = new SqlParameter("session_name", SqlDbType.NVarChar, 256);
                    param.Value = _sessionName;
                    cmd.Parameters.Add(param);
                    SqlDataReader reader = cmd.ExecuteReader();
                    XEPosition pos = new XEPosition();
                    if(reader.HasRows)
                    {
                        pos.LastFile = reader["session_file_name"].ToString();
                        pos.Offset = Convert.ToInt64(reader["session_offset"]);
                    }
                    return pos;
                }
            }
        }

        void IStore.Update(XEPosition pos)
        {
            using (SqlConnection conn = _sqlContext.Connection)
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "dbo.xetohub_save_or_update_offset";
                    SqlParameter param = new SqlParameter("session_name", SqlDbType.NVarChar, 256);
                    param.Value = _sessionName;
                    cmd.Parameters.Add(param);

                    SqlParameter param1 = new SqlParameter("session_file_name", SqlDbType.NVarChar, 260);
                    param1.Value = pos.LastFile;
                    cmd.Parameters.Add(param1);

                    SqlParameter param2 = new SqlParameter("session_offset", SqlDbType.BigInt);
                    param2.Value = pos.Offset;
                    cmd.Parameters.Add(param2);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
