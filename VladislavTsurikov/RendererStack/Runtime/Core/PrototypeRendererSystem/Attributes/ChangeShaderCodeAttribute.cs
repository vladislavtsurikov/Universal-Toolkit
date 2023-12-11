using System;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.SelectionDatas;

namespace VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public abstract class ChangeShaderCodeAttribute : Attribute
    {
        public abstract void ChangeShaderCode(Prototype proto);
    }
}