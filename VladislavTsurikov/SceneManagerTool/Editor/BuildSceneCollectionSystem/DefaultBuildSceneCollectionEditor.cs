#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Attributes;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.SceneManagerTool.Editor.SceneCollectionSystem;
using VladislavTsurikov.SceneManagerTool.Runtime.BuildSceneCollectionSystem.Components;

namespace VladislavTsurikov.SceneManagerTool.Editor.BuildSceneCollectionSystem
{
    [ElementEditor(typeof(DefaultBuildSceneCollection))]
    public class DefaultBuildSceneCollectionEditor : IMGUIElementEditor
    {
        protected SceneCollectionStackEditor _sceneCollectionStackEditor;
        
        protected DefaultBuildSceneCollection _buildSceneCollection;

        public override void OnEnable()
        {
            _buildSceneCollection = (DefaultBuildSceneCollection)Target;
            _sceneCollectionStackEditor = new SceneCollectionStackEditor(new GUIContent("Scene Collections"), _buildSceneCollection.SceneCollectionList);
        }

        public override void OnGUI()
        {
            _sceneCollectionStackEditor.OnGUI();
        }
    }
}
#endif