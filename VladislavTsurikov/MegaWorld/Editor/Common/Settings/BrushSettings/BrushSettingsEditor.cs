#if UNITY_EDITOR
using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Attributes;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.BrushSettings;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.BrushSettings
{
    [ElementEditor(typeof(Runtime.Common.Settings.BrushSettings.BrushSettings))]
    public class BrushSettingsEditor : IMGUIElementEditor
    {
	    private Runtime.Common.Settings.BrushSettings.BrushSettings _brushSettings;
	    private ProceduralMaskEditor _proceduralMaskEditor;
	    private CustomMasksEditor _customMasksEditor;

	    private readonly BrushJitterSettingsEditor _brushSettingsJitterSettingsEditor = new BrushJitterSettingsEditor();

        public override void OnEnable()
        {
	        _brushSettings = (Runtime.Common.Settings.BrushSettings.BrushSettings)Target; 
	        _proceduralMaskEditor = new ProceduralMaskEditor(_brushSettings.ProceduralMask);
	        _customMasksEditor = new CustomMasksEditor(_brushSettings.CustomMasks);
        }

        public override void OnGUI()
        {
	        _brushSettings.SpacingEqualsType = (SpacingEqualsType)CustomEditorGUILayout.EnumPopup(_spacingEqualsType, _brushSettings.SpacingEqualsType);

	        if(_brushSettings.SpacingEqualsType == SpacingEqualsType.Custom)
	        {
		        _brushSettings.CustomSpacing = CustomEditorGUILayout.FloatField(_spacing, _brushSettings.CustomSpacing);
	        }

	        _brushSettings.MaskType = (MaskType)CustomEditorGUILayout.EnumPopup(_maskType, _brushSettings.MaskType);
			
	        switch (_brushSettings.MaskType)
	        {
		        case MaskType.Custom:
		        {
			        _customMasksEditor.OnGUI();

			        break;
		        }
		        case MaskType.Procedural:
		        {
			        _proceduralMaskEditor.OnGUI();

			        break;
		        }
	        }

	        _brushSettingsJitterSettingsEditor.OnGUI(_brushSettings, _brushSettings.BrushJitterSettings);
        }

        [NonSerialized]
		private readonly GUIContent _spacingEqualsType = new GUIContent("Spacing Equals", "Allows you to set what size the Spacing will be.");
		[NonSerialized]
		private readonly GUIContent _spacing = new GUIContent("Spacing", "Controls the distance between _brushSettings marks.");
		[NonSerialized]
		private readonly GUIContent _maskType = new GUIContent("Mask Type", "Allows you to choose which _brushSettings mask will be used.");
    }
}
#endif
