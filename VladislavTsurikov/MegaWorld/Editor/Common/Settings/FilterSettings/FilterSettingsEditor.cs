#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Attributes;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.MegaWorld.Editor.Common.Settings.FilterSettings.MaskFilterSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.FilterSettings
{
    [ElementEditor(typeof(Runtime.Common.Settings.FilterSettings.FilterSettings))]
    public class FilterSettingsEditor : IMGUIElementEditor
    {
        private Runtime.Common.Settings.FilterSettings.FilterSettings _filterSettings;

        private SimpleFilterEditor _simpleFilterEditor;
        private MaskFilterStackElementEditor _maskFilterStackElementEditor;
        
        public override void OnEnable()
        {
            _filterSettings = (Runtime.Common.Settings.FilterSettings.FilterSettings)Target;

            _simpleFilterEditor = new SimpleFilterEditor();
            _simpleFilterEditor.Init(_filterSettings.SimpleFilter);
            _maskFilterStackElementEditor = new MaskFilterStackElementEditor();
            _maskFilterStackElementEditor.Init(_filterSettings.MaskFilterComponentSettings);
        }
        
        public override void OnGUI()
        {
            _filterSettings.FilterType = (FilterType)CustomEditorGUILayout.EnumPopup(new GUIContent("Filter Type"), _filterSettings.FilterType);
            
            switch (_filterSettings.FilterType)
            {
                case FilterType.SimpleFilter:
                {
                    _simpleFilterEditor.OnGUI();
                    break;
                }
                case FilterType.MaskFilter:
                {
                    CustomEditorGUILayout.HelpBox("\"Mask Filter\" works only with Unity terrain");
                    _maskFilterStackElementEditor.OnGUI();
                    break;
                }
            }
        }
    }
}
#endif