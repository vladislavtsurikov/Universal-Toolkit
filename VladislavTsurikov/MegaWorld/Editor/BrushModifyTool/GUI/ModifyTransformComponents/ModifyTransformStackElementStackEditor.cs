#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Attributes;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.MegaWorld.Editor.BrushModifyTool.ModifyTransformComponents;

namespace VladislavTsurikov.MegaWorld.Editor.BrushModifyTool.GUI.ModifyTransformComponents
{
    [ElementEditor(typeof(ModifyTransformSettings))]
    public class ModifyTransformStackElementStackEditor : IMGUIElementEditor
    {
        private ModifyTransformSettings _settings;
        
        private ModifyTransformStackEditor _modifyTransformStackEditor;

        public override void OnEnable()
        {
            _settings = (ModifyTransformSettings)Target;
            _modifyTransformStackEditor = new ModifyTransformStackEditor(new GUIContent("Modify Transform Components"), _settings.Stack);
        }

        public override void OnGUI() 
        {
            _modifyTransformStackEditor.OnGUI();
        }
    }
}
#endif