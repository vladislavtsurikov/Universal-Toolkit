using System;

namespace VladislavTsurikov.Core.Runtime.IconStack.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class MissingIconsWarningAttribute : Attribute
    {
        public readonly string Text;

        public MissingIconsWarningAttribute(string text)
        {
            Text = text;
        }
    }
}