#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings
{
    [DontDrawFoldout]
    [ElementEditor(typeof(SuccessSettings))]
    public class SuccessSettingsEditor : IMGUIElementEditor
    {
        private SuccessSettings _successSettings => (SuccessSettings)Target;
        
        public override void OnGUI()
        {
            _successSettings.SuccessValue = CustomEditorGUILayout.Slider(_success, _successSettings.SuccessValue, 0f, 100f);
        }

		private GUIContent _success = new GUIContent("Success (%)");
    }
}
#endif
