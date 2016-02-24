namespace xetohub.core.xevent
{
    public class XEPayload
    {

        public XEPayload(System.Collections.Generic.Dictionary<string, object> ht, XEPosition pos)
        {
            this.Dictionary = ht;
            this.Position = pos;
        }

        //public XEvent Event { get; set;  }
        public XEPosition Position { get; set; }

        public System.Collections.Generic.Dictionary<string, object> Dictionary { get; set; }
    }
}