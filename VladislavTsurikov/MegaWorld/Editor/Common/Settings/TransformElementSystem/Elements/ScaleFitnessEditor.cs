#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Attributes;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem.Components;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.TransformElementSystem.Elements
{
    [ElementEditor(typeof(ScaleFitness))]
    public class ScaleFitnessEditor : ReorderableListComponentEditor
    {
        private ScaleFitness _scaleFitness;
        public override void OnEnable()
        {
            _scaleFitness = (ScaleFitness)Target;
        }
        
        public override void OnGUI(Rect rect, int index) 
        {
            _scaleFitness.OffsetScale = EditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), 
                new GUIContent("Offset Scale"), _scaleFitness.OffsetScale);
            rect.y += EditorGUIUtility.singleLineHeight;
        }

        public override float GetElementHeight(int index) 
        {   
            float height = EditorGUIUtility.singleLineHeight;

            height += EditorGUIUtility.singleLineHeight;

            return height;
        }
    }
}
#endif