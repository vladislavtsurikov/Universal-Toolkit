using System.Collections.Generic;
using VladislavTsurikov.DeepCopy.Runtime;

namespace VladislavTsurikov.ComponentStack.Runtime
{
    public class Clipboard<T> where T : Component
    {
        private List<T> _copiedElementList = new List<T>();
        private ComponentStack<T> _stack;

        public Clipboard(ComponentStack<T> stack)
        {
            _stack = stack;
        }

        public void CopyAllSettings()
        {
            _copiedElementList.Clear();

            foreach (var component in _stack.ElementList)
            {
                _copiedElementList.Add(DeepCopier.Copy(component));  
            }
        }
    }
}