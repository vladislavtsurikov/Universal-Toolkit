using VladislavTsurikov.ComponentStack.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.Core;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.DefaultComponentsSystem
{
    public class DefaultGroupComponent : Component
    {
        protected Group Group;

        protected override void SetupElement(object[] args = null)
        {
            Group = (Group)args[0];
        }
    }
}