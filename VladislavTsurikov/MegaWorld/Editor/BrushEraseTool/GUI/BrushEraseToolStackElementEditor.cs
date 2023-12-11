#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Attributes;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList.Attributes;

namespace VladislavTsurikov.MegaWorld.Editor.BrushEraseTool.GUI
{
    [DontDrawFoldout]
    [ElementEditor(typeof(BrushEraseToolSettings))]
    public class BrushEraseToolStackElementEditor : IMGUIElementEditor
    {
        private BrushEraseToolSettings _brushEraseToolSettings;
        
        public override void OnEnable()
        {
            _brushEraseToolSettings = (BrushEraseToolSettings)Target;
        }

        public override void OnGUI() 
        {
            _brushEraseToolSettings.EraseStrength = CustomEditorGUILayout.Slider(new GUIContent("Erase Strength"), _brushEraseToolSettings.EraseStrength, 0, 1);
        }
    }
}
#endif