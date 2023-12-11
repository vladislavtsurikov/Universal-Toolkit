using System;

namespace VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class DontDrawForAddButton : Attribute
    {
    }
}