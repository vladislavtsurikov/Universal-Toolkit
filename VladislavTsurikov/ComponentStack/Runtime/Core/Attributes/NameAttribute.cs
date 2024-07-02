using System;

namespace VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class NameAttribute : Attribute
    {
        public readonly string Name;

        public NameAttribute(string name)
        {
            Name = name;
        }
    }
}