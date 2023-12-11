#if SCENE_MANAGER_ADDRESSABLES
using System.Collections;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using VladislavTsurikov.SceneUtility.Runtime;

namespace VladislavTsurikov.SceneUtility.Scripts.Integration.Addressables
{
    public class AddressablesSceneOperations : SceneOperations
    {
        private AsyncOperationHandle<SceneInstance> _loadSceneAsyncOperation;
        private AsyncOperationHandle<SceneInstance> _unloadSceneAsyncOperation;

        public AddressablesSceneOperations(SceneReference sceneReference) : base(sceneReference)
        {
        }
        
        public override float LoadingProgress()
        {
            if (_loadSceneAsyncOperation.IsValid())
                return _loadSceneAsyncOperation.PercentComplete;

            return SceneReference.Scene.isLoaded ? 1 : 0;
        }
        
        protected override IEnumerator LoadSceneOverride()
        {
            _loadSceneAsyncOperation = UnityEngine.AddressableAssets.Addressables.LoadSceneAsync(SceneReference.ScenePath, LoadSceneMode.Additive);
            yield return _loadSceneAsyncOperation;
        }
        
        protected override IEnumerator UnloadSceneOverride()
        {
            _unloadSceneAsyncOperation = UnityEngine.AddressableAssets.Addressables.UnloadSceneAsync(_loadSceneAsyncOperation);
            yield return _unloadSceneAsyncOperation;
        }

        public override bool IsLoading()
        {
            if (!_loadSceneAsyncOperation.IsValid())
            {
                return false;
            }

            return _loadSceneAsyncOperation.PercentComplete != 1;
        }

        public override bool IsUnloading()
        {
            if (!_unloadSceneAsyncOperation.IsValid())
                return false;

            return _unloadSceneAsyncOperation.PercentComplete != 1;
        }
    }
}
#endif