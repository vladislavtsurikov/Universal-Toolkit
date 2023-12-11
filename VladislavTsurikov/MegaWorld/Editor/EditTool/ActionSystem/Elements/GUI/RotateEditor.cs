#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Attributes;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.MegaWorld.Runtime.Common;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.Utility.Runtime;

namespace VladislavTsurikov.MegaWorld.Editor.EditTool.ActionSystem.Elements.GUI
{
	[ElementEditor(typeof(Rotate))]
	public class RotateEditor : IMGUIElementEditor
    {
	    private Rotate _settings;
	    public override void OnEnable()
	    {
		    _settings = (Rotate)Target;
	    }
	    
	    public override void OnGUI()
        {
	        _settings.MouseSensitivitySettings.MouseSensitivity = CustomEditorGUILayout.Slider(_mouseSensitivity, _settings.MouseSensitivitySettings.MouseSensitivity, 
				MouseSensitivitySettings.MinMouseSensitivity, MouseSensitivitySettings.MaxMouseSensitivity);

	        GlobalCommonComponentSingleton<TransformSpaceSettings>.Instance.TransformSpace = (TransformSpace)CustomEditorGUILayout.EnumPopup(_transformSpace, GlobalCommonComponentSingleton<TransformSpaceSettings>.Instance.TransformSpace);

            _settings.EnableSnapRotate = CustomEditorGUILayout.Toggle(new GUIContent("Enable Snap Rotate"), _settings.EnableSnapRotate);
			if(_settings.EnableSnapRotate)
			{
				EditorGUI.indentLevel++;
				_settings.SnapRotate = Mathf.Max(CustomEditorGUILayout.FloatField(new GUIContent("Snap Rotate"), _settings.SnapRotate), 0.001f);
				EditorGUI.indentLevel--;
			}
        }
        
		private readonly GUIContent _transformSpace = new GUIContent("Transform Space", "Changes from local to global space.");
		private readonly GUIContent _mouseSensitivity = new GUIContent("Mouse Sensitivity", "Сhanges the strength of the transform modification with the mouse.");
	}
}
#endif