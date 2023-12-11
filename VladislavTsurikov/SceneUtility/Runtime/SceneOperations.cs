using System.Collections;

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

        public IEnumerator LoadScene()
        {
            if (SceneReference.Scene.isLoaded || IsLoading())
                yield break;

            yield return LoadSceneOverride();
        }

        public IEnumerator UnloadScene()
        {
            if (!SceneReference.Scene.isLoaded || IsUnloading())
                yield break;
            
            yield return UnloadSceneOverride();
        }
        
        protected abstract IEnumerator LoadSceneOverride();
        protected abstract IEnumerator UnloadSceneOverride();
        public abstract float LoadingProgress();

        public abstract bool IsLoading();
        public abstract bool IsUnloading();
    }
}