﻿#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Attributes;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.SceneManagerTool.Editor.SettingsSystem.OperationSystem;
using VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem;
using VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem.Components;

namespace VladislavTsurikov.SceneManagerTool.Editor.SettingsSystem.Components
{
    [ElementEditor(typeof(BeforeUnloadOperationsSettings))]
    public class BeforeUnloadOperationsSettingsEditor : ReorderableListComponentEditor
    {
        private BeforeUnloadOperationsSettings _beforeUnloadOperationsSettings;
        private SceneOperationStackEditor _sceneOperationStackEditor;

        public override void OnEnable()
        {
            _beforeUnloadOperationsSettings = (BeforeUnloadOperationsSettings)Target;
            _sceneOperationStackEditor = new SceneOperationStackEditor(SettingsTypes.BeforeUnloadScene, _beforeUnloadOperationsSettings.OperationList);
        }

        public override void OnGUI(Rect rect, int index)
        {
            _sceneOperationStackEditor.OnGUI(rect);
        }
        
        public override float GetElementHeight(int index)
        {
            float height = 0;

            height += _sceneOperationStackEditor.GetElementStackHeight();

            return height;
        }
    }
}
#endif