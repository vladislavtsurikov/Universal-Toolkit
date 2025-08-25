﻿#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.ReflectionUtility.Runtime;

namespace VladislavTsurikov.ComponentStack.Editor.Core
{
    public static class AllEditorTypes<T> where T: Element
    {
        public static Dictionary<Type, Type> Types = new Dictionary<Type, Type>(); // ElementType => EditorType

        static AllEditorTypes()
        {
            List<Type> componentTypes = AllTypesDerivedFrom<T>.Types.ToList();

            var editorTypes = AllTypesDerivedFrom<ElementEditor>.Types
                .Where(
                    t => t.IsDefined(typeof(ElementEditorAttribute), false)
                         && !t.IsAbstract
                );
            
            foreach (var type in editorTypes)
            {
                var attribute = type.GetAttribute<ElementEditorAttribute>();

                if (componentTypes.Contains(attribute.SettingsType))
                {
                    if (!Types.Keys.Contains(attribute.SettingsType))
                    {
                        Types.Add(attribute.SettingsType, type);

                        componentTypes.Remove(attribute.SettingsType);
                    }
                }
            }
        }
    }
}
#endif