using System.Collections;
using System.Collections.Generic;
using VladislavTsurikov.ComponentStack.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.SceneManagerTool.Runtime.SceneCollectionSystem;
using VladislavTsurikov.SceneUtility.Runtime;

namespace VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem.OperationSystem
{
    public class Operation : Component
    {
        public virtual IEnumerator DoOperation()
        {
            yield break;
        }

        public virtual List<SceneReference> GetSceneReferences()
        {
            return new List<SceneReference>();
        }
    }
}