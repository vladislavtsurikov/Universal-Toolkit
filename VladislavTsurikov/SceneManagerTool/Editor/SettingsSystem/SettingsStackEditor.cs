#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ComponentStack.Editor;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem;

namespace VladislavTsurikov.SceneManagerTool.Editor.SettingsSystem
{
    public class SettingsStackEditor : ReorderableListStackEditor<SettingsComponent, ReorderableListComponentEditor>
    {
        private readonly bool _sceneCollection;
        
        private ComponentStackOnlyDifferentTypes<SettingsComponent> ComponentStackOnlyDifferentTypes => (ComponentStackOnlyDifferentTypes<SettingsComponent>)Stack;
        
        public SettingsStackEditor(GUIContent reorderableListName, bool sceneCollection, ComponentStackOnlyDifferentTypes<SettingsComponent> list) : base(reorderableListName, list, true)
        {
            _sceneCollection = sceneCollection;
            CopySettings = true;
            ShowActiveToggle = false;
        }

        protected override void ShowAddMenu()
        {
            GenericMenu menu = new GenericMenu();

            foreach (var type in AllEditorTypes<SettingsComponent>.Types)
            {
                Type settingsType = type.Key;
                
                bool exists = ComponentStackOnlyDifferentTypes.GetElement(settingsType) != null;
                
                string context = settingsType.GetAttribute<NameAttribute>().Name;

                if (_sceneCollection)
                {
                    if(settingsType.GetAttribute<SceneComponentAttribute>() != null)
                    {
                        continue;
                    }
                }
                else
                {
                    if(settingsType.GetAttribute<SceneCollectionComponentAttribute>() != null)
                    {
                        continue;
                    }
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