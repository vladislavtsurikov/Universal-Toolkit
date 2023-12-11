#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Attributes;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem.Components;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.TransformElementSystem.Elements
{
    [ElementEditor(typeof(TreeRotation))]
    public class TreeRotationEditor : ReorderableListComponentEditor
    {
        private TreeRotation _treeRotation;
        public override void OnEnable()
        {
            _treeRotation = (TreeRotation)Target;
        }
        
        public override void OnGUI(Rect rect, int index) 
        {
            _treeRotation.RandomizeOrientationY = EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), 
                new GUIContent("Randomize Y (%)"), _treeRotation.RandomizeOrientationY, 0.0f, 100.0f);
            rect.y += EditorGUIUtility.singleLineHeight;
            _treeRotation.RandomizeOrientationXZ = EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), 
                new GUIContent("Randomize XZ (%)"), _treeRotation.RandomizeOrientationXZ, 0.0f, 50.0f);
            rect.y += EditorGUIUtility.singleLineHeight;

            _treeRotation.SuccessRandomizeOrientationXZ = EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), 
                new GUIContent("Success Randomize XZ (%)"), _treeRotation.SuccessRandomizeOrientationXZ, 0.0f, 100.0f);
            rect.y += EditorGUIUtility.singleLineHeight;
        }

        public override float GetElementHeight(int index) 
        {
            float height = EditorGUIUtility.singleLineHeight;

            height += EditorGUIUtility.singleLineHeight;
            height += EditorGUIUtility.singleLineHeight;
            height += EditorGUIUtility.singleLineHeight;

            return height;
        }
    }
}
#endif