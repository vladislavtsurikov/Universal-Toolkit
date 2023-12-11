using System;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.ElementsSystem.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AddGeneralPrototypeComponentsAttribute : AddComponentsAttribute
    {
        public AddGeneralPrototypeComponentsAttribute(Type[] prototypeTypes, Type[] types) : base(prototypeTypes, types)
        {
        }

        public AddGeneralPrototypeComponentsAttribute(Type prototypeType, Type[] types) : base(prototypeType, types)
        {
        }
    }
}