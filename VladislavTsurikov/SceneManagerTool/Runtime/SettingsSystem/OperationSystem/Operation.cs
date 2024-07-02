using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.ComponentStack.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.SceneManagerTool.Runtime.SceneCollectionSystem;
using VladislavTsurikov.SceneUtility.Runtime;

namespace VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem.OperationSystem
{
    public class Operation : Component
    {
        public virtual async UniTask DoOperation() { }

        public virtual List<SceneReference> GetSceneReferences()
        {
            return new List<SceneReference>();
        }
    }
}