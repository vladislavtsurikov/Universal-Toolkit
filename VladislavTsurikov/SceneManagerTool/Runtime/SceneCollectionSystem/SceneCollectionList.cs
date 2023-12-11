using System.Collections.Generic;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.SceneUtility.Runtime;

namespace VladislavTsurikov.SceneManagerTool.Runtime.SceneCollectionSystem
{
    public class SceneCollectionList : ComponentStackSupportSameType<SceneCollection>
    {
        public List<SceneReference> GetSceneReferences()
        {
            List<SceneReference> sceneReferences = new List<SceneReference>();

            foreach (var sceneCollection in ElementList)
            {
                sceneReferences.AddRange(sceneCollection.GetSceneReferences());
            }
            
            return sceneReferences;
        }
    }
}