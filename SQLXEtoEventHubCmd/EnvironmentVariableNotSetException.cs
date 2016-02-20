using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLXEtoEventHubCmd
{
    public class EnvironmentVariableNotSetException : Exception
    {
        public string RequiredVariable { get; set;  }

        public EnvironmentVariableNotSetException(string RequiredVariable)
        {
            this.RequiredVariable = RequiredVariable;
        }

        public override string Message
        {
            get
            {
                return string.Format("Environment variable {0:S} must be exported and valid", RequiredVariable);
            }
        }
    }
}
