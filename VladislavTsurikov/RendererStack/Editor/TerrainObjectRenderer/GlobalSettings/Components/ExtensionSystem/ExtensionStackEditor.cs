#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ComponentStack.Editor;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList.Attributes;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.GlobalSettings.Components.ExtensionSystem;

namespace VladislavTsurikov.RendererStack.Editor.TerrainObjectRenderer.GlobalSettings.Components.ExtensionSystem
{
    public class ExtensionStackEditor : ReorderableListStackEditor<Extension, ReorderableListComponentEditor>
    {
        private readonly ComponentStackOnlyDifferentTypes<Extension> _componentStackOnlyDifferentTypes;
        public ExtensionStackEditor(ComponentStackOnlyDifferentTypes<Extension> list) : base(new GUIContent(), list, true)
        {
            _componentStackOnlyDifferentTypes = list;
        }

        protected override void ShowAddMenu()
        {
            GenericMenu menu = new GenericMenu();

            foreach (var item in AllEditorTypes<Extension>.Types)
            {
                System.Type extensionType = item.Key;

                string context = extensionType.GetAttribute<MenuItemAttribute>().Name;

                ContextMenuAttribute contextMenuAttribute = item.Value.GetAttribute<ContextMenuAttribute>();

                if (contextMenuAttribute != null)
                {
                    context = contextMenuAttribute.ContextMenu;
                }
                
                if (_componentStackOnlyDifferentTypes.GetElement(extensionType) == null)
                {
                    menu.AddItem(new GUIContent(context), false, () => _componentStackOnlyDifferentTypes.CreateIfMissingType(extensionType));
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