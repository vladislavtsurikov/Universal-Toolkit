using System;

namespace VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class MenuItemAttribute : Attribute
    {
        public readonly string Name;

        public MenuItemAttribute(string name)
        {
            Name = name;
        }
    }
}