using VladislavTsurikov.ComponentStack.Runtime.Interfaces;
using VladislavTsurikov.Core.Runtime;

namespace VladislavTsurikov.ComponentStack.Runtime
{
    public class AdvancedElementList<T> : CallbackList<T> 
        where T: IRemoved
    {
        public AdvancedElementList()
        {
            OnRemoved += OnRemove;
        }
        
        private void OnRemove(int index)
        {
            this[index].OnRemove();
        }
    }
}