using System.Collections;
using System.Collections.Generic;
using System.Linq;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.SceneManagerTool.Runtime.SceneCollectionSystem;
using VladislavTsurikov.SceneUtility.Runtime;

namespace VladislavTsurikov.SceneManagerTool.Runtime.SceneTypeSystem
{
    [MenuItem("Group")]
    public class Group : SceneType
    {
        public List<SceneReference> SceneReferences = new List<SceneReference>();
        
        protected override IEnumerator Load()
        {
            foreach (var sceneReference in SceneReferences)
            {
                yield return sceneReference.LoadScene();
            }
        }

        protected override IEnumerator Unload(SceneCollection nextLoadSceneCollection)
        {
            foreach (var sceneReference in SceneReferences)
            {
                yield return UnloadSceneReference(nextLoadSceneCollection, sceneReference);
            }
            
            yield return null;
        }
        
        public override bool HasScene(SceneReference sceneReference)
        {
            return SceneReferences.FindAll(reference => reference.SceneName == sceneReference.SceneName).Count != 0;
        }

        protected override List<SceneReference> GetSceneReferences()
        {
            return new List<SceneReference>(SceneReferences);
        }

        public override float LoadingProgress()
        {
            return !SceneReferences.Any() ? 1 : SceneReferences.Sum(a => a.LoadingProgress) / SceneReferences.Count;
        }
    }
}