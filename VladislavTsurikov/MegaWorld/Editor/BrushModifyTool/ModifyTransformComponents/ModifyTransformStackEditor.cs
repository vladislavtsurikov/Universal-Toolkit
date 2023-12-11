#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ComponentStack.Editor;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;

namespace VladislavTsurikov.MegaWorld.Editor.BrushModifyTool.ModifyTransformComponents
{
    public class ModifyTransformStackEditor : ReorderableListStackEditor<ModifyTransformComponent, ReorderableListComponentEditor>
    {
        private ComponentStackOnlyDifferentTypes<ModifyTransformComponent> ComponentStackOnlyDifferentTypes => (ComponentStackOnlyDifferentTypes<ModifyTransformComponent>)Stack;
        
        public ModifyTransformStackEditor(GUIContent label, ComponentStackOnlyDifferentTypes<ModifyTransformComponent> list) : base(label, list, true)
        {
            DisplayHeaderText = false;
        }

        protected override void ShowAddMenu()
        {
            GenericMenu menu = new GenericMenu();

            foreach (var type in AllEditorTypes<ModifyTransformComponent>.Types)
            {
                Type modifyTransformComponentType = type.Key;
                
                string context = modifyTransformComponentType.GetAttribute<MenuItemAttribute>().Name;

                bool exists = ComponentStackOnlyDifferentTypes.HasType(modifyTransformComponentType);

                if (!exists)
                {
                    menu.AddItem(new GUIContent(context), false, () => ComponentStackOnlyDifferentTypes.CreateIfMissingType(modifyTransformComponentType));
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent(context));
                }
            }

            menu.ShowAsContext();
        }
    }
}
#endif