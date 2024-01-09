#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Attributes;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.FilterSettings.MaskFilterSystem
{
    [ElementEditor(typeof(MaskFilterComponentSettings))]
    public class MaskFilterComponentSettingsEditor : IMGUIElementEditor
    {
        private MaskFilterComponentSettings _maskFilterComponentSettings;
        private MaskFilterStackEditor _maskFilterStackEditor;
        
        public static bool ChangedGUI;
        
        public override void OnEnable()
        {
            _maskFilterComponentSettings = (MaskFilterComponentSettings)Target;
            _maskFilterStackEditor = new MaskFilterStackEditor(new GUIContent("Mask Filters Settings"), _maskFilterComponentSettings.MaskFilterStack);
        }

        public override void OnGUI() 
        {
            EditorGUI.BeginChangeCheck();

            _maskFilterStackEditor.OnGUI();
			
            if(EditorGUI.EndChangeCheck())
            {
                ChangedGUI = true;
            }
        }
    }
}
#endif