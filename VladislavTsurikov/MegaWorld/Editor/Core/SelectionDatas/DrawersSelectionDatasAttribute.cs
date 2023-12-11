using System;

namespace VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DrawersSelectionDatasAttribute : Attribute
    {
        public readonly Type SelectionPrototypeWindowType;
        public readonly Type SelectionGroupWindowType;
        
        public DrawersSelectionDatasAttribute(Type selectionPrototypeWindowType, Type selectionGroupWindowType)
        {
            SelectionPrototypeWindowType = selectionPrototypeWindowType;
            SelectionGroupWindowType = selectionGroupWindowType;
        }

        public DrawersSelectionDatasAttribute(Type selectionPrototypeWindowType)
        {
            SelectionPrototypeWindowType = selectionPrototypeWindowType;
        }
    }
}