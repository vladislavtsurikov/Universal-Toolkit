﻿#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem;
using VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem.OperationSystem;
using VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem.OperationSystem.Attributes;

namespace VladislavTsurikov.SceneManagerTool.Editor.SettingsSystem.OperationSystem
{
    public class SceneOperationStackEditor : ReorderableListStackEditor<Operation, ReorderableListComponentEditor>
    {
        private readonly SettingsTypes _settingsTypes;
        private readonly ComponentStackSupportSameType<Operation> _componentStackSupportSameType;
        
        public SceneOperationStackEditor(SettingsTypes settingsTypes, ComponentStackSupportSameType<Operation> list) : base(new GUIContent("Settings"), list, true)
        {
            _componentStackSupportSameType = list;
            _settingsTypes = settingsTypes;
            CopySettings = true;
            ShowActiveToggle = false;
        }

        protected override void ShowAddMenu()
        {
            GenericMenu menu = new GenericMenu();

            foreach (var settingsType in GetSettingsTypeForAddManu())
            {
                switch (_settingsTypes)
                {
                    case SettingsTypes.AfterLoadScene:
                        if(settingsType.GetAttribute<AfterLoadScene>() == null)
                            continue;
                        break;
                    case SettingsTypes.BeforeLoadScene:
                        if(settingsType.GetAttribute<BeforeLoadScene>() == null)
                            continue;
                        break;
                    case SettingsTypes.AfterUnloadScene:
                        if(settingsType.GetAttribute<AfterUnloadScene>() == null)
                            continue;
                        break;
                    case SettingsTypes.BeforeUnloadScene:
                        if(settingsType.GetAttribute<BeforeUnloadScene>() == null)
                            continue;
                        break;
                }

                string context = settingsType.GetAttribute<MenuItemAttribute>().Name;

                menu.AddItem(new GUIContent(context), false, () => _componentStackSupportSameType.CreateComponent(settingsType));
            }

            menu.ShowAsContext();
        }
    }
}
#endif