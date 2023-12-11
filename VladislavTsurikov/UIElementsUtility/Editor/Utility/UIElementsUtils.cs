#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;
using VladislavTsurikov.Utility.Runtime.Extensions;

namespace VladislavTsurikov.UIElementsUtility.Editor.Utility
{
    public static class UIElementsUtils
    {
        public static void AddClass(string className, IEnumerable<VisualElement> elements, bool removeNulls = true)
        {
            if (className.IsNullOrEmpty())
                return;

            if (elements == null)
                return;

            if (removeNulls)
                elements = elements.Where(item => item != null);

            foreach (VisualElement element in elements)
                element.AddClass(className);
        }

        public static void RemoveClass(string className, IEnumerable<VisualElement> elements, bool removeNulls = true)
        {
            if (className.IsNullOrEmpty())
                return;

            if (elements == null)
                return;

            if (removeNulls)
                elements = elements.Where(item => item != null);

            foreach (VisualElement element in elements)
                element.RemoveClass(className);
        }
    }
}
#endif