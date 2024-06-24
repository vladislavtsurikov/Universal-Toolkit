using System.Collections;
using System.Collections.Generic;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.SceneManagerTool.Runtime.SceneCollectionSystem;
using VladislavTsurikov.SceneUtility.Runtime;

namespace VladislavTsurikov.SceneManagerTool.Runtime.SceneTypeSystem
{
    [MenuItem("Single Scene")]
    public class Single : SceneType
    {
        public SceneReference SceneReference = new SceneReference();

        public override string Name
        {
            get
            {
                if (!SceneReference.IsValid())
                {
                    return "Set Scene [Single Scene]";
                }
                else
                {
                    return SceneReference.SceneName + " [Single Scene]";
                }
            }
        }
        
        protected override IEnumerator Load()
        {
            yield return SceneReference.LoadScene();
        }

        protected override IEnumerator Unload(SceneCollection nextLoadSceneCollection)   
        {
            yield return UnloadSceneReference(nextLoadSceneCollection, SceneReference);
        }
        
        public override bool HasScene(SceneReference sceneReference)
        {
            return SceneReference.SceneName == sceneReference.SceneName;
        }

        protected override List<SceneReference> GetSceneReferences()
        {
            return new List<SceneReference>{SceneReference};
        }

        public override float LoadingProgress()
        {
            return SceneReference.LoadingProgress;
        }
    }
}