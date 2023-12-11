using System;

namespace VladislavTsurikov.ComponentStack.Editor.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class ElementEditorAttribute : Attribute
    {
        public readonly Type SettingsType;

        public ElementEditorAttribute(Type settingsType)
        {
            SettingsType = settingsType;
        }
    }
}