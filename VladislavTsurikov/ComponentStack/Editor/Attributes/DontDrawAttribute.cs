using System;

namespace VladislavTsurikov.ComponentStack.Editor.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class DontDrawAttribute : Attribute
    {
    }
}