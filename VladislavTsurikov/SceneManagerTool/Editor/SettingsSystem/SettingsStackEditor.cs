#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ComponentStack.Editor;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem;
using VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem.Attributes;

namespace VladislavTsurikov.SceneManagerTool.Editor.SettingsSystem
{
    public class SettingsStackEditor : ReorderableListStackEditor<SettingsComponentElement, ReorderableListComponentEditor>
    {
        private readonly bool _sceneCollection;
        
        private ComponentStackOnlyDifferentTypes<SettingsComponentElement> ComponentStackOnlyDifferentTypes => (ComponentStackOnlyDifferentTypes<SettingsComponentElement>)Stack;
        
        public SettingsStackEditor(GUIContent reorderableListName, bool sceneCollection, ComponentStackOnlyDifferentTypes<SettingsComponentElement> list) : base(reorderableListName, list, true)
        {
            _sceneCollection = sceneCollection;
            CopySettings = true;
            ShowActiveToggle = false;
        }

        protected override void ShowAddMenu()
        {
            GenericMenu menu = new GenericMenu();

            foreach (var type in AllEditorTypes<SettingsComponentElement>.Types)
            {
                Type settingsType = type.Key;
                
                bool exists = ComponentStackOnlyDifferentTypes.GetElement(settingsType) != null;
                
                string context = settingsType.GetAttribute<MenuItemAttribute>().Name;

                if (_sceneCollection)
                {
                    if(settingsType.GetAttribute<SceneAttribute>() != null)
                        continue;
                }
                else
                {
                    if(settingsType.GetAttribute<SceneCollectionAttribute>() != null)
                        continue;
                }
                
                if (!exists)
                {
                    menu.AddItem(new GUIContent(context), false, () => ComponentStackOnlyDifferentTypes.CreateIfMissingType(settingsType));
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