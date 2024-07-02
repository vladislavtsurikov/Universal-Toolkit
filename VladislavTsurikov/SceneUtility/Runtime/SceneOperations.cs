using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace VladislavTsurikov.SceneUtility.Runtime
{
    public abstract class SceneOperations
    {
        public bool Enable = true;
        
        protected readonly SceneReference SceneReference;
        
        public SceneOperations(SceneReference sceneReference)
        {
            SceneReference = sceneReference;
        }

        internal async UniTask LoadSceneInternal()
        {
            if (SceneReference.Scene.isLoaded || IsLoading())
            {
                return;
            }

            await LoadScene();
        }

        internal async UniTask UnloadSceneInternal()
        {
            if (!SceneReference.Scene.isLoaded || IsUnloading())
            {
                return;
            }
            
            await UnloadScene();
        }
        
        protected abstract UniTask LoadScene();
        protected abstract UniTask UnloadScene();
        public abstract float LoadingProgress();
        public abstract bool IsLoading();
        public abstract bool IsUnloading();
    }
}