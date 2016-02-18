using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace SQLXEtoEventHub.XEPosition
{
    public class RegistryStore : IStore
    {
        protected const string KEY_PATH = "SOFTWARE";
        protected const string KEY_NODE = "SQLXEtoEventHub";

        protected const string LASTFILE = "LastFile";
        protected const string OFFSET = "Offset";

        public string Trace { get; protected set; }

        public RegistryStore(string traceName)
        {
            this.Trace = traceName;
        }

        public void Update(XEPosition pos)
        {
            var root = Registry.CurrentUser.OpenSubKey(KEY_PATH, RegistryKeyPermissionCheck.ReadWriteSubTree);
            var key = root.CreateSubKey(KEY_NODE, RegistryKeyPermissionCheck.ReadWriteSubTree);
            if (key == null) throw new UnauthorizedAccessException("Registry key not created. Please run the installer or create it manually");
            var trace = key.CreateSubKey(Trace);

            trace.SetValue(LASTFILE, pos.LastFile, RegistryValueKind.String);
            trace.SetValue(OFFSET, pos.Offset, RegistryValueKind.QWord);
        }

        public XEPosition Read()
        {
            var root = Registry.CurrentUser.OpenSubKey(KEY_PATH);
            var key = root.OpenSubKey(KEY_NODE);
            if (key == null) return new XEPosition();
            var trace = key.OpenSubKey(Trace);
            if (trace == null) return new XEPosition();

            return new XEPosition
            {
                LastFile = (string)trace.GetValue(LASTFILE),
                Offset = (long)trace.GetValue(OFFSET)
            };
        }
    }
}
