#if UNITY_EDITOR
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ActionFlow.Runtime.Actions;

namespace VladislavTsurikov.ActionFlow.Editor
{
    [CustomEditor(typeof(AssetActions), true)]
    public class AssetActionsEditor : UnityEditor.Editor
    {
        private AssetActions _settings;
        
        private ReorderableListStackEditor<Action, ReorderableListComponentEditor> _actionsCollectionEditor;

        private void OnEnable()
        {
            _settings = (AssetActions)target;
            
            _actionsCollectionEditor = new ReorderableListStackEditor<Action, ReorderableListComponentEditor>(
                new GUIContent("Actions"), _settings.ActionCollection, true);
        }
        
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            
            _actionsCollectionEditor.OnGUI();
            
            EditorGUILayout.Space();
            _settings.CancellationActions = (AssetActions)EditorGUILayout.ObjectField(
                new GUIContent("Cancellation Actions"), 
                _settings.CancellationActions, 
                typeof(AssetActions), 
                false);

            if(EditorGUI.EndChangeCheck())
            {
                _settings.SetDirtyAssetActions();
            }
        }
    }
}
#endif