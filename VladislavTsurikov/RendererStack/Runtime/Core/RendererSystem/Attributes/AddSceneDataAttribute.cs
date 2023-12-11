﻿using System;

namespace VladislavTsurikov.RendererStack.Runtime.Core.RendererSystem.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class AddSceneDataAttribute : Attribute
    {
        public readonly Type[] Types;

        public AddSceneDataAttribute(Type[] types)
        {
            Types = types;
        }
    }
}