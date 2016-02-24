namespace xetohub.sql
{
    public class XESession
    {
        public string Name { get; private set; }
        public string FilePath { get; private set; }

        public XESession()
        {
        }
        public XESession(string name, string filePath)
        {
            this.Name = name;
            this.FilePath = filePath;
        }
    }
}