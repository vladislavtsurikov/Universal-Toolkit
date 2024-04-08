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
    public class BeforeLoadOperationsSettings : SettingsComponent
    {
        public ComponentStackSupportSameType<Operation> OperationStack = new ComponentStackSupportSameType<Operation>();

        public IEnumerator DoOperations()
        {
            foreach (var sceneOperation in OperationStack.ElementList)
            {
                yield return sceneOperation.DoOperation();
            }
        }
        
        public override List<SceneReference> GetSceneReferences()
        {
            List<SceneReference> sceneReferences =
                new List<SceneReference>();

            foreach (var operation in OperationStack.ElementList)
            {
                sceneReferences.AddRange(operation.GetSceneReferences());
            }

            return sceneReferences;
        }
    }
}