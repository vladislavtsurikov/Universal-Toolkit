#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.PhysicsSimulatorEditor.Editor;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.PhysicsToolsSettings
{
    public static class PositionOffsetSettingsEditor 
    {
	    private static bool _positionOffsetFoldout = true;

        public static void OnGUI(PositionOffsetSettings settings)
        {
			_positionOffsetFoldout = CustomEditorGUILayout.Foldout(_positionOffsetFoldout, "Position Offset Settings");

			if (_positionOffsetFoldout)
			{
				EditorGUI.indentLevel++;

				settings.EnableAutoOffset = CustomEditorGUILayout.Toggle(new GUIContent("Enable Auto Offset"), settings.EnableAutoOffset);
                settings.PositionOffsetDown = CustomEditorGUILayout.Slider(new GUIContent("Position Offset Down (%)"), settings.PositionOffsetDown, 0, 100);
                
				EditorGUI.indentLevel--;
			}
        }
    }
}
#endif