using System;

namespace VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class AddSceneComponentsAttribute : Attribute
    {
        public readonly Type[] Types;

        public AddSceneComponentsAttribute(Type[] types)
        {
            Types = types;
        }
    }
}