#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.MegaWorld.Editor.BrushModifyTool.ModifyTransformComponents;

namespace VladislavTsurikov.MegaWorld.Editor.BrushModifyTool.GUI.ModifyTransformComponents
{
	[ElementEditor(typeof(RandomizeScale))]
    public class RandomizeScaleEditor : ReorderableListComponentEditor
    {
	    private RandomizeScale _settings;

	    public override void OnEnable()
	    {
		    _settings = (RandomizeScale)Target;
	    }
	    
        public override void OnGUI(Rect rect, int index) 
        {
	        _settings.UniformScale = EditorGUI.Toggle(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Uniform Scale"), _settings.UniformScale);
            rect.y += EditorGUIUtility.singleLineHeight;

            if(_settings.UniformScale)
			{
				float minSameScaleValue = _settings.MinScale.x;
				float maxSameScaleValue = _settings.MaxScale.x;

                GUIStyle alignmentStyleRight = new GUIStyle(UnityEngine.GUI.skin.label)
                {
	                alignment = TextAnchor.MiddleRight,
	                stretchWidth = true
                };
                GUIStyle alignmentStyleLeft = new GUIStyle(UnityEngine.GUI.skin.label)
                {
	                alignment = TextAnchor.MiddleLeft,
	                stretchWidth = true
                };
                GUIStyle alignmentStyleCenter = new GUIStyle(UnityEngine.GUI.skin.label)
                {
	                alignment = TextAnchor.MiddleCenter,
	                stretchWidth = true
                };

                EditorGUI.MinMaxSlider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Scale"), ref minSameScaleValue, ref maxSameScaleValue, 0f, 5f);
                rect.y += EditorGUIUtility.singleLineHeight * 0.5f;
                Rect slopeLabelRect = new Rect(rect.x + EditorGUIUtility.labelWidth, rect.y, (rect.width - EditorGUIUtility.labelWidth) * 0.2f, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(slopeLabelRect, "0", alignmentStyleLeft);
                slopeLabelRect = new Rect(rect.x + EditorGUIUtility.labelWidth + (rect.width - EditorGUIUtility.labelWidth) * 0.2f, rect.y, (rect.width - EditorGUIUtility.labelWidth) * 0.6f, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(slopeLabelRect, "2.5", alignmentStyleCenter);
                slopeLabelRect = new Rect(rect.x + EditorGUIUtility.labelWidth + (rect.width - EditorGUIUtility.labelWidth) * 0.8f, rect.y, (rect.width - EditorGUIUtility.labelWidth) * 0.2f, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(slopeLabelRect, "5", alignmentStyleRight);
                rect.y += EditorGUIUtility.singleLineHeight;
    
                //Label
                EditorGUI.LabelField(new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight), new GUIContent(""));
                //Min Label
                Rect numFieldRect = new Rect(rect.x + EditorGUIUtility.labelWidth, rect.y, (rect.width - EditorGUIUtility.labelWidth) * 0.2f, EditorGUIUtility.singleLineHeight);
                GUIContent minContent = new GUIContent("");
    
                EditorGUI.LabelField(numFieldRect, minContent, alignmentStyleLeft);
                numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);
    
                minSameScaleValue = EditorGUI.FloatField(numFieldRect, minSameScaleValue);
                numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);
                
                EditorGUI.LabelField(numFieldRect, " ");
                numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);
    
                maxSameScaleValue = EditorGUI.FloatField(numFieldRect, maxSameScaleValue);
                numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);
    
                GUIContent maxContent = new GUIContent("");
                EditorGUI.LabelField(numFieldRect, maxContent, alignmentStyleRight);
    
                rect.y += EditorGUIUtility.singleLineHeight;

                _settings.MinScale = new Vector3(minSameScaleValue, minSameScaleValue, minSameScaleValue);
                _settings.MaxScale = new Vector3(maxSameScaleValue, maxSameScaleValue, maxSameScaleValue);
			}
			else
			{
				_settings.MinScale = EditorGUI.Vector3Field(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Min Scale"), _settings.MinScale);
                rect.y += EditorGUIUtility.singleLineHeight;
                rect.y += EditorGUIUtility.singleLineHeight;
                _settings.MaxScale = EditorGUI.Vector3Field(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Max Scale"), _settings.MaxScale);
                rect.y += EditorGUIUtility.singleLineHeight;
                rect.y += EditorGUIUtility.singleLineHeight;
			}
        }

        public override float GetElementHeight(int index) 
        {
            float height = EditorGUIUtility.singleLineHeight;

            height += EditorGUIUtility.singleLineHeight;

            if(_settings.UniformScale)
			{
                height += EditorGUIUtility.singleLineHeight;
                height += EditorGUIUtility.singleLineHeight;
                height += EditorGUIUtility.singleLineHeight;                
			}
			else
			{
                height += EditorGUIUtility.singleLineHeight;
                height += EditorGUIUtility.singleLineHeight;
                height += EditorGUIUtility.singleLineHeight;
                height += EditorGUIUtility.singleLineHeight;
			}

            return height;
        }
    }
}
#endif