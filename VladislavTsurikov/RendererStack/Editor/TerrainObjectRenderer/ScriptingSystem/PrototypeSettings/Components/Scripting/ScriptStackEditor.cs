#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList.Attributes;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.ScriptingSystem.PrototypeSettings.Scripting;

namespace VladislavTsurikov.RendererStack.Editor.TerrainObjectRenderer.ScriptingSystem.PrototypeSettings.Scripting
{
    public class ScriptStackEditor : ReorderableListStackEditor<Script, ReorderableListComponentEditor>
    {
        private readonly ComponentStackSupportSameType<Script> _componentStackSupportSameType;
        public ScriptStackEditor(ComponentStackSupportSameType<Script> list) : base(new GUIContent(""), list, true)
        {
            _componentStackSupportSameType = list;
        }
        
        protected override void ShowAddMenu()
        {
            GenericMenu menu = new GenericMenu();

            foreach (var item in AllEditorTypes<Script>.Types)
            {
                System.Type extensionType = item.Key;

                string context = extensionType.GetAttribute<MenuItemAttribute>().Name;

                ContextMenuAttribute contextMenuAttribute = item.Value.GetAttribute<ContextMenuAttribute>();

                if (contextMenuAttribute != null)
                {
                    context = contextMenuAttribute.ContextMenu;
                }
                
                if (_componentStackSupportSameType.GetElement(extensionType) == null)
                {
                    menu.AddItem(new GUIContent(context), false, () => _componentStackSupportSameType.CreateComponent(extensionType));
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