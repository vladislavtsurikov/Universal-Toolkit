#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Attributes;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.SceneManagerTool.Editor.SettingsSystem.OperationSystem;
using VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem;
using VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem.Components;

namespace VladislavTsurikov.SceneManagerTool.Editor.SettingsSystem.Components
{
    [ElementEditor(typeof(AfterLoadOperationsSettings))]
    public class AfterLoadOperationsSettingsEditor : ReorderableListComponentEditor
    {
        private AfterLoadOperationsSettings _afterLoadOperationsSettings;
        private SceneOperationStackEditor _sceneOperationStackEditor;

        public override void OnEnable()
        {
            _afterLoadOperationsSettings = (AfterLoadOperationsSettings)Target;
            _sceneOperationStackEditor = new SceneOperationStackEditor(SettingsTypes.AfterLoadScene, _afterLoadOperationsSettings.OperationStack);
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