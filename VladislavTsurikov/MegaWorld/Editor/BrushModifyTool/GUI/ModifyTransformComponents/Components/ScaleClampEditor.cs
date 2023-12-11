#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Attributes;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.MegaWorld.Editor.BrushModifyTool.ModifyTransformComponents.Elements;

namespace VladislavTsurikov.MegaWorld.Editor.BrushModifyTool.GUI.ModifyTransformComponents.Components
{
    [ElementEditor(typeof(ScaleClamp))]
    public class ScaleClampEditor : ReorderableListComponentEditor
    {
        private ScaleClamp _settings;

        public override void OnEnable()
        {
            _settings = (ScaleClamp)Target;
        }
        
        public override void OnGUI(Rect rect, int index) 
        {
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

            EditorGUI.MinMaxSlider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Scale Clamp", "Allows you to set the clamp when the Scale value is not less or more than certain values."), ref _settings.MinScale, ref _settings.MaxScale, 0f, 5f);
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
    
            _settings.MinScale = EditorGUI.FloatField(numFieldRect, _settings.MinScale);
            numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);
            
            EditorGUI.LabelField(numFieldRect, " ");
            numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);
    
            _settings.MaxScale = EditorGUI.FloatField(numFieldRect, _settings.MaxScale);
            numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);
    
            GUIContent maxContent = new GUIContent("");
            EditorGUI.LabelField(numFieldRect, maxContent, alignmentStyleRight);
        }

        public override float GetElementHeight(int index) 
        {
            float height = EditorGUIUtility.singleLineHeight;

            height += EditorGUIUtility.singleLineHeight * 2;

            return height;
        }
    }
}
#endif