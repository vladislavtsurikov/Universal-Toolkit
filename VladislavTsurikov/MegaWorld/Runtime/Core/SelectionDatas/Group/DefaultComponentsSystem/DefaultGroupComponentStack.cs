using System.Linq;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ComponentStack.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.OdinSerializer.Core.Misc;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.DefaultComponentsSystem
{
    public class DefaultGroupComponentStack : ComponentStackOnlyDifferentTypes<Component>
    {
        [OdinSerialize]
        private Group _group;

        protected override void OnSetup()
        {
            _group = (Group)InitializationDataForElements[0];
        }

        protected override void OnCreateElements()
        {
            AddDefaultGroupComponentsAttribute addDefaultGroupComponentsAttribute = _group.PrototypeType.GetAttribute<AddDefaultGroupComponentsAttribute>();

            if (addDefaultGroupComponentsAttribute == null)
            {
                return;
            }

            CreateIfMissingType(addDefaultGroupComponentsAttribute.Types);
        }

        public override void OnRemoveInvalidElements()
        {
            AddDefaultGroupComponentsAttribute addDefaultGroupComponentsAttribute = _group.PrototypeType.GetAttribute<AddDefaultGroupComponentsAttribute>();

            if (addDefaultGroupComponentsAttribute == null)
            {
                RemoveAll();
                return;
            }
            
            for (int i = ElementList.Count - 1; i >= 0; i--)
            {
                if (!addDefaultGroupComponentsAttribute.Types.Contains(ElementList[i].GetType()))
                {
                    Remove(i);
                }
            }
        }
    }
}