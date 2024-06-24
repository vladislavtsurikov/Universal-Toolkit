namespace VladislavTsurikov.ComponentStack.Runtime.Core
{
    public interface ISetup
    {
        void Setup(object[] args = null, bool force = false);
    }
}