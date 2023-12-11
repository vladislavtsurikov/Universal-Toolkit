using System;

namespace VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class PrototypeTypeAttribute : Attribute
    {
        public readonly Type Type;

        public PrototypeTypeAttribute(Type type)
        {
            Type = type;
        }
    }
}