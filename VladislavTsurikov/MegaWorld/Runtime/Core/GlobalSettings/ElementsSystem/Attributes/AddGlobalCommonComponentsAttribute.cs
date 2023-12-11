﻿using System;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AddGlobalCommonComponentsAttribute : Attribute
    {
        public readonly Type[] Types;
        
        public AddGlobalCommonComponentsAttribute(Type[] types)
        {
            Types = types;
        }
    }
}