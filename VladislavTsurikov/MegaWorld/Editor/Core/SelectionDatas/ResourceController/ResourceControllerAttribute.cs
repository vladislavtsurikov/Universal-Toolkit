using System;

namespace VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas.ResourceController
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ResourceControllerAttribute : Attribute
    {
        public readonly Type PrototypeType;

        public ResourceControllerAttribute(Type prototypeType)
        {
            PrototypeType = prototypeType;
        }
    }
}