using xetohub.core.xevent;

namespace xetohub.core.store
{
    public interface IStore
    {
        void Update(XEPosition pos);
        XEPosition Read();
    }
}
