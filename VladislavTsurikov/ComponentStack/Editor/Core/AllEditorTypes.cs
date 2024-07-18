#if UNITY_EDITOR
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
            var elementTypes = AllTypesDerivedFrom<T>.Types;

            var editorTypes = AllTypesDerivedFrom<ElementEditor>.Types
                .Where(
                    t => t.IsDefined(typeof(ElementEditorAttribute), false)
                         && !t.IsAbstract
                );
            
            foreach (var type in editorTypes)
            {
                var attribute = type.GetAttribute<ElementEditorAttribute>();

                if (elementTypes.Contains(attribute.SettingsType))
                {
                    if (!Types.Keys.Contains(attribute.SettingsType))
                    {
                        Types.Add(attribute.SettingsType, type);
                    }
                }
                else if(attribute.SettingsType.ToString() == typeof(T).ToString())
                {
                    Types.Add(attribute.SettingsType, type);
                }
            }
        }
    }
}
#endif