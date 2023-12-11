using System;

namespace VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Components.Camera.CameraSettingsSystem.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class AddCameraComponentsAttribute : Attribute
    {
        public readonly Type[] Types;

        public AddCameraComponentsAttribute(Type[] types)
        {
            Types = types;
        }
    }
}