using System;
using System.Collections.Generic;

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
        
        public T CreateComponent(Type type)
        {
            return Create(type);
        }
    }
}