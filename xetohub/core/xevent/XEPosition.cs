using System;

namespace xetohub.core.xevent
{
    public class XEPosition
    {
        public string LastFile { get; set; }
        public long Offset { get; set; }

        public XEPosition()
        {
            LastFile = string.Empty;
            Offset = 0;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is XEPosition))
                throw new ArgumentException(string.Format("Cannot compare a {0:S} with a XEPosition", obj.GetType().FullName));
            XEPosition x2 = (XEPosition)obj;

            return this.LastFile.Equals(x2.LastFile) && this.Offset.Equals(x2.Offset);
        }

        public override int GetHashCode()
        {
            return LastFile.GetHashCode() ^ Offset.GetHashCode();
        }

        public override string ToString()
        {
            return this.GetType().Name + string.Format("[LastFile=\"{0:S}\", Offset={1:N0}]", LastFile, Offset);
        }
    }

}