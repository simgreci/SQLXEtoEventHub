namespace SQLXEtoEventHub.Store
{
    public interface IStore
    {
        void Update(XEvent.XEPosition pos);
        XEvent.XEPosition Read();
    }
}
