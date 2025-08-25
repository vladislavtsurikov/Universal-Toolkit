using System.Linq;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;
using Core_Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;
using Runtime_Core_Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.MonoBehaviour
{
    public class ComponentStack : ComponentStackOnlyDifferentTypes<Runtime_Core_Component>
    {
        private UnityEngine.MonoBehaviour _tool;

        protected override void OnSetup()
        {
            _tool = (UnityEngine.MonoBehaviour)SetupData[0]; 
        }

        protected override void OnCreateElements()
        {
            AddMonoBehaviourComponentsAttribute addMonoBehaviourComponentsAttribute = (AddMonoBehaviourComponentsAttribute)_tool.GetType().GetAttribute(typeof(AddMonoBehaviourComponentsAttribute));

            if (addMonoBehaviourComponentsAttribute == null)
            {
                return;
            }

            CreateIfMissingType(addMonoBehaviourComponentsAttribute.Types);
        }
        
        public override void OnRemoveInvalidElements()
        {
            for (int i = ElementList.Count - 1; i >= 0; i--)
            {
                AddMonoBehaviourComponentsAttribute addMonoBehaviourComponentsAttribute = (AddMonoBehaviourComponentsAttribute)_tool.GetType().GetAttribute(typeof(AddMonoBehaviourComponentsAttribute));

                if (addMonoBehaviourComponentsAttribute == null)
                {
                    Remove(i);
                    continue;
                }

                if (!addMonoBehaviourComponentsAttribute.Types.Contains(ElementList[i].GetType()))
                {
                    Remove(i);
                }
            }
        }
    }
}