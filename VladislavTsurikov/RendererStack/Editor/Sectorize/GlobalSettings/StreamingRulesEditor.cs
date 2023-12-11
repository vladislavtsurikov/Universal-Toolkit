#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Attributes;
using VladislavTsurikov.Core.Runtime;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.RendererStack.Runtime.Sectorize.GlobalSettings;

namespace VladislavTsurikov.RendererStack.Editor.Sectorize.GlobalSettings
{
    [ElementEditor(typeof(StreamingRules))]
    public class StreamingRulesEditor : IMGUIElementEditor
    {
        private StreamingRules _streamingRules;

        public override void OnEnable() 
        {
            _streamingRules = (StreamingRules)Target;
        }

        public override void OnGUI()
        {
            float initialLabelWidth = CustomEditorGUILayout.LabelWidth;
            CustomEditorGUILayout.LabelWidth = 290;
                
            _streamingRules.MaxImmediatelyLoadingDistance = Mathf.Max(0, CustomEditorGUILayout.FloatField(_maxImmediatelyLoadingDistance, _streamingRules.MaxImmediatelyLoadingDistance));
            _streamingRules.OffsetMaxLoadingDistanceWithPause = Mathf.Max(0, CustomEditorGUILayout.FloatField(_offsetMaxLoadingDistanceWithPause, _streamingRules.OffsetMaxLoadingDistanceWithPause)); 
            
            if (_streamingRules.OffsetMaxLoadingDistanceWithPause != 0)
            {
                EditorGUI.indentLevel++;
                _streamingRules.MaxPauseBeforeLoadingScene = Mathf.Max(0, CustomEditorGUILayout.FloatField(_maxPauseBeforeLoadingScene, _streamingRules.MaxPauseBeforeLoadingScene));
                EditorGUI.indentLevel--;
            }
            
            _streamingRules.OffsetMaxDistancePreventingUnloadScene = Mathf.Max(0, CustomEditorGUILayout.FloatField(_offsetMaxDistancePreventingUnloadScene, _streamingRules.OffsetMaxDistancePreventingUnloadScene)); 
            
            _streamingRules.UseCaching = CustomEditorGUILayout.Toggle(_useCachingg, _streamingRules.UseCaching);
            if (_streamingRules.UseCaching)
            {
                EditorGUI.indentLevel++;

                if (_streamingRules.OffsetMaxLoadingDistanceWithPause != 0)
                {
                    _streamingRules.MaxPauseBeforeLoadingCachedScene = Mathf.Max(0, CustomEditorGUILayout.FloatField(_maxPauseBeforeLoadingCachedScene, _streamingRules.MaxPauseBeforeLoadingCachedScene));
                }
                
                _streamingRules.KeepScenes = Mathf.Max(0, CustomEditorGUILayout.FloatField(_keepScenes, _streamingRules.KeepScenes));
                StreamingUtilitySettings.Instance.CacheMemoryThreshold = (CacheMemoryThreshold)CustomEditorGUILayout.EnumPopup(_cacheMemoryThreshold, StreamingUtilitySettings.Instance.CacheMemoryThreshold);

                if (StreamingUtilitySettings.Instance.CacheMemoryThreshold == CacheMemoryThreshold.Custom)
                {
                    EditorGUI.indentLevel++;
                    StreamingUtilitySettings.Instance.CustomCacheMemoryThreshold = CustomEditorGUILayout.FloatField( new GUIContent("Threshold In Megabyte"), StreamingUtilitySettings.Instance.CustomCacheMemoryThreshold);
                    EditorGUI.indentLevel--;
                }
                
                EditorGUI.indentLevel--;
            }

            CustomEditorGUILayout.LabelWidth = initialLabelWidth;
        }

        private GUIContent _maxImmediatelyLoadingDistance = new GUIContent("Max Immediately Loading Distance", "Sets the distance from the camera where scenes will be loaded immediately in one frame. " +
            "It's better not to make this value too big, otherwise when the camera moves you will have to load too many scenes in one frame, which will cause a lot of delay, I advise you to use the Offset Max Loading Distance With Pause parameter, this will not load scenes immediately in one frame");
        private GUIContent _offsetMaxLoadingDistanceWithPause = new GUIContent("Offset Max Loading Distance With Pause", "Loads scenes not in one frame, first the scene is loaded, then a pause for a few seconds, then the next scene is loaded. This parameter is necessary to make loading scenes as invisible to the player as possible.");

        private GUIContent _offsetMaxDistancePreventingUnloadScene = new GUIContent("Offset Max Distance Preventing Unload Scene");
        private GUIContent _maxPauseBeforeLoadingScene = new GUIContent("Max Pause Before Loading Scene", "Pause in seconds until the next attempt to load the scene");
        private GUIContent _maxPauseBeforeLoadingCachedScene = new GUIContent("Max Pause Before Loading Cached Scene", "Pause in seconds until the next attempt to load the cached scene");
        private GUIContent _keepScenes = new GUIContent("Keep Scenes", "Cached scenes will be unloaded in seconds starting when the scene was cached");
        private GUIContent _cacheMemoryThreshold = new GUIContent("Cache Memory Threshold", "RAM threshold where scenes will no longer be cached, but will be unloaded.");
        private GUIContent _useCachingg = new GUIContent("Use Caching", "The scene becomes inactive instead of unloading");
    }
}
#endif