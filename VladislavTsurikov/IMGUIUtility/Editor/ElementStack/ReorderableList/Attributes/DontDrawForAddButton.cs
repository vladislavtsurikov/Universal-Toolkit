using System;

namespace VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class DontDrawForAddButton : Attribute
    {
    }
}