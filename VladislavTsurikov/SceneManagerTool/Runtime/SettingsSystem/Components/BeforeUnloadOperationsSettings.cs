using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem.OperationSystem;
using VladislavTsurikov.SceneUtility.Runtime;

namespace VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem
{
    [Name("Before Unload")]
    public class BeforeUnloadOperationsSettings : SettingsComponent
    {
        public ComponentStackSupportSameType<Operation> OperationStack = new ComponentStackSupportSameType<Operation>();
        
        public async UniTask DoOperations()
        {
            foreach (var sceneOperation in OperationStack.ElementList)
            {
                await sceneOperation.DoOperation();
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