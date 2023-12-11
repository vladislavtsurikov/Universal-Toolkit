using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VladislavTsurikov.SceneUtility.Runtime
{
    public class SceneManagerSceneOperations : SceneOperations
    {
        private AsyncOperation _loadSceneAsyncOperation;
        private AsyncOperation _unloadSceneAsyncOperation;
        
        public SceneManagerSceneOperations(SceneReference sceneReference) : base(sceneReference)
        {
        }
        
        public override float LoadingProgress()
        {
            if (_loadSceneAsyncOperation != null)
                return _loadSceneAsyncOperation.progress;

            return SceneReference.Scene.isLoaded ? 1 : 0;
        }

        protected override IEnumerator LoadSceneOverride()
        {
            _loadSceneAsyncOperation = SceneManager.LoadSceneAsync(SceneReference.SceneName, LoadSceneMode.Additive);

            yield return _loadSceneAsyncOperation;
            
        }

        protected override IEnumerator UnloadSceneOverride()
        {
            _unloadSceneAsyncOperation = SceneManager.UnloadSceneAsync(SceneReference.SceneName);

            yield return _unloadSceneAsyncOperation;
        }
        
        public override bool IsLoading()
        {
            if (_loadSceneAsyncOperation == null)
            {
                return false;
            }

            return _loadSceneAsyncOperation.progress != 1;
        }

        public override bool IsUnloading()
        {
            if (_unloadSceneAsyncOperation == null)
                return false;

            return _unloadSceneAsyncOperation.progress != 1;
        }
    }
}