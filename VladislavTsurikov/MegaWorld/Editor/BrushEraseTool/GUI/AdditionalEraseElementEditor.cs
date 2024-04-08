#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Attributes;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList.Attributes;
using VladislavTsurikov.MegaWorld.Editor.BrushEraseTool.PrototypeElements;

namespace VladislavTsurikov.MegaWorld.Editor.BrushEraseTool.GUI
{
    [DontDrawFoldout]
    [ElementEditor(typeof(AdditionalEraseSetting))]
    public class AdditionalEraseElementEditor : IMGUIElementEditor
    {
        private AdditionalEraseSetting _additionalEraseSetting;
        
        public override void OnEnable()
        {
            _additionalEraseSetting = (AdditionalEraseSetting)Target;
        }

        public override void OnGUI()
        {
            _additionalEraseSetting.Success = CustomEditorGUILayout.Slider(_success, _additionalEraseSetting.Success, 0f, 100f);

        }

        private readonly GUIContent _success = new GUIContent("Success of Erase (%)");
    }
}
#endif