#if UNITY_EDITOR
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ActionFlow.Runtime.Actions;

namespace VladislavTsurikov.ActionFlow.Editor
{
    [CustomEditor(typeof(Actions))]
    public class ActionsEditor : UnityEditor.Editor
    {
        private Actions Actions => (Actions)target;

        private ReorderableListStackEditor<Action, ReorderableListComponentEditor> _actionsCollectionEditor;
        
        public void OnEnable()
        {
            _actionsCollectionEditor = new ReorderableListStackEditor<Action, ReorderableListComponentEditor>(
                new GUIContent("Actions"), Actions.ActionCollection, true);
        }

        public override void OnInspectorGUI()
        {
            _actionsCollectionEditor.OnGUI();
        }
    }
}
#endif