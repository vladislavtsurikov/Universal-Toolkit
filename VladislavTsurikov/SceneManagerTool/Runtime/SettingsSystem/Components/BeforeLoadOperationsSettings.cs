using System.Collections;
using System.Collections.Generic;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.SceneManagerTool.Runtime.SceneCollectionSystem;
using VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem.OperationSystem;
using VladislavTsurikov.SceneUtility.Runtime;

namespace VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem.Components
{
    [MenuItem("Before Load")]
    public class BeforeLoadOperationsSettings : SettingsComponentElement
    {
        public ComponentStackSupportSameType<Operation> OperationList = new ComponentStackSupportSameType<Operation>();

        public IEnumerator DoOperations(SceneCollection loadSceneCollection)
        {
            foreach (var sceneOperation in OperationList.ElementList)
            {
                yield return sceneOperation.DoOperation(loadSceneCollection);
            }
        }
        
        public override List<SceneReference> GetSceneReferences()
        {
            List<SceneReference> sceneReferences =
                new List<SceneReference>();

            foreach (var operation in OperationList.ElementList)
            {
                sceneReferences.AddRange(operation.GetSceneReferences());
            }

            return sceneReferences;
        }
    }
}