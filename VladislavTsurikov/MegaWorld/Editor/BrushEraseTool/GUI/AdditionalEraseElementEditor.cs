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
    [ElementEditor(typeof(AdditionalEraseElement))]
    public class AdditionalEraseElementEditor : IMGUIElementEditor
    {
        private AdditionalEraseElement _additionalEraseElement;
        
        public override void OnEnable()
        {
            _additionalEraseElement = (AdditionalEraseElement)Target;
        }

        public override void OnGUI()
        {
            _additionalEraseElement.Success = CustomEditorGUILayout.Slider(_success, _additionalEraseElement.Success, 0f, 100f);

        }

        private readonly GUIContent _success = new GUIContent("Success of Erase (%)");
    }
}
#endif