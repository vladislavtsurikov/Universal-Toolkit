#if UNITY_EDITOR
using System;
using UnityEngine;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.MegaWorld.Editor.Core.Window;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.BrushSettings;
using VladislavTsurikov.MegaWorld.Runtime.Core.PreferencesSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainDetail;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainTexture;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.BrushSettings
{
    [Serializable]
    public class BrushJitterSettingsEditor 
    {
        public void OnGUI(Runtime.Common.Settings.BrushSettings.BrushSettings brush, BrushJitterSettings jitter)
        {
            brush.BrushSize = CustomEditorGUILayout.Slider(_brushSize, brush.BrushSize, 0.1f, PreferenceElementSingleton<BrushPreferenceSettings>.Instance.MaxBrushSize);

            jitter.BrushSizeJitter = CustomEditorGUILayout.Slider(_brushJitter, jitter.BrushSizeJitter, 0f, 1f);

			//CustomEditorGUILayout.Separator();

			jitter.BrushScatter = CustomEditorGUILayout.Slider(_brushScatter, jitter.BrushScatter, 0f, 1f);
            jitter.BrushScatterJitter = CustomEditorGUILayout.Slider(_brushJitter, jitter.BrushScatterJitter, 0f, 1f);

			//CustomEditorGUILayout.Separator();

			if(WindowData.Instance.SelectedData.HasOneSelectedGroup())
			{
				if(WindowData.Instance.SelectedData.SelectedGroup.PrototypeType == typeof(PrototypeTerrainDetail) || 
				   WindowData.Instance.SelectedData.SelectedGroup.PrototypeType == typeof(PrototypeTerrainTexture))
				{
					brush.BrushRotation = CustomEditorGUILayout.Slider(_brushRotation, brush.BrushRotation, -180f, 180f);
            		jitter.BrushRotationJitter = CustomEditorGUILayout.Slider(_brushJitter, jitter.BrushRotationJitter, 0f, 1f);

					//CustomEditorGUILayout.Separator();
				}
			}
        }

		[NonSerialized]
        private GUIContent _brushSize = new GUIContent("Brush Size", "Selected prototypes will only spawn in this range around the center of Brush.");
		[NonSerialized]
		private GUIContent _brushJitter = new GUIContent("Jitter", "Control brush stroke randomness.");
		[NonSerialized]
		private GUIContent _brushScatter = new GUIContent("Brush Scatter", "Randomize brush position by an offset.");
		[NonSerialized]
		private GUIContent _brushRotation = new GUIContent("Brush Rotation", "Rotation of the brush.");
    }
}
#endif