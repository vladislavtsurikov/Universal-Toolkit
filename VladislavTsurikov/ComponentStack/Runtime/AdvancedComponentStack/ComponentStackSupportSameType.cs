using System;
using System.Collections.Generic;
using VladislavTsurikov.ComponentStack.Runtime.Core;

namespace VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack
{
    public class ComponentStackSupportSameType<T> : AdvancedComponentStack<T>
        where T : Component
    {
        public void CreateComponentsIfMissingType(Type[] types)
        {
            CreateElementIfMissingType(types);
        }

        public T CreateComponentIfMissingType(Type type)
        {
            return CreateElementIfMissingType(type);
        }
        
        public void CreateComponents(List<Type> types)
        {
            foreach (var type in types)
            {
                Create(type);
            }
        }
        
        public T CreateComponent(Type type, int index = -1)
        {
            return Create(type, index);
        }
        
        public List<T> GetElementsOfType(Type type)
        {
            List<T> elements = new List<T>();
            for (int i = 0; i < _elementList.Count; i++)
            {
                if (_elementList[i] != null && _elementList[i].GetType() == type)
                {
                    elements.Add(_elementList[i]);
                }
            }
            return elements;
        }
    }
}