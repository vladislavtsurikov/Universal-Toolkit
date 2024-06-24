using System;
using System.Collections.Generic;

namespace VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CreateComponentsAttribute : Attribute
    {
        public readonly List<Type> Types;

        public CreateComponentsAttribute(Type[] types)
        {
            Types = new List<Type>(types);
        }
    }
}