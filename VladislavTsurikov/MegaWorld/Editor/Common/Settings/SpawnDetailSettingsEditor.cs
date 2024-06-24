#if UNITY_EDITOR
using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings
{
	[Serializable]
    [ElementEditor(typeof(SpawnDetailSettings))]
    public class SpawnDetailSettingsEditor : IMGUIElementEditor
    {
	    private SpawnDetailSettings _spawnDetailSettings;

	    public override void OnEnable()
        {
	        _spawnDetailSettings = (SpawnDetailSettings)Target;
        }

        public override void OnGUI()
        {
	        _spawnDetailSettings.UseRandomOpacity = CustomEditorGUILayout.Toggle(_useRandomOpacity, _spawnDetailSettings.UseRandomOpacity);
	        _spawnDetailSettings.Density = CustomEditorGUILayout.IntSlider(_density, _spawnDetailSettings.Density, 0, 10);
	        _spawnDetailSettings.FailureRate = CustomEditorGUILayout.Slider(_failureRate, _spawnDetailSettings.FailureRate, 0f, 100f);
        }

		private GUIContent _density = new GUIContent("Density");
		private GUIContent _useRandomOpacity = new GUIContent("Use Random Opacity");
		private GUIContent _failureRate = new GUIContent("Failure Rate (%)", "The larger this value, the less likely it is to spawn an object.");
    }
}
#endif
