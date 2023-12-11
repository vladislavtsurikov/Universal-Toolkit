using System;

namespace VladislavTsurikov.ComponentStack.Runtime.Attributes
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