using System;

namespace VladislavTsurikov.ComponentStack.Runtime.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class DontCreateAttribute : Attribute
    {
    }
}