using System;
using System.Collections.Generic;

namespace VladislavTsurikov.Core.Runtime.IconStack.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class DropObjectsAttribute : Attribute
    {
        public readonly List<Type> ObjectsTypes = new List<Type>();

        public DropObjectsAttribute(Type type)
        {
            ObjectsTypes.Add(type);
        }

        public DropObjectsAttribute(Type[] types)
        {
            ObjectsTypes.AddRange(types);
        }
    }
}