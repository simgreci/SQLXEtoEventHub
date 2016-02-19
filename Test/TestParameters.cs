using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public class TestParameters
    {
        public static string CONNECTION_STRING
        {
            get
            {
                return Environment.GetEnvironmentVariable("EH_SQL_CONNECTION_STRING");
            }
        }

        public static string XESESSION_NAME
        {
            get
            {
                return Environment.GetEnvironmentVariable("EH_XESESSION_NAME");
            }
        }

        public static string XE_PATH
        {
            get
            {
                return Environment.GetEnvironmentVariable("EH_XESESSION_PATH");
            }
        }
    }
}
