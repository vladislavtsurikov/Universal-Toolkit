using System.Collections.Generic;
using VladislavTsurikov.ComponentStack.Runtime;
using VladislavTsurikov.SceneUtility.Runtime;

namespace VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem
{
    public abstract class SettingsComponentElement : Component
    {
        public virtual List<SceneReference> GetSceneReferences()
        {
            return new List<SceneReference>();
        }
    }
}