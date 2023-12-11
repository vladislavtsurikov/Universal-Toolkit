#if UNITY_EDITOR
using System;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;

namespace VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas.ResourceController
{
    public class ResourceControllerEditorStack : ComponentStackOnlyDifferentTypes<ResourceControllerEditor>
    {
        protected override void OnCreateElements()
        {
            CreateAllElementTypes();
        }

        public ResourceControllerEditor GetResourceControllerEditor(Type prototypeType)
        {
            foreach (var editor in _elementList)
            {
                ResourceControllerAttribute resourceControllerAttribute =
                    (ResourceControllerAttribute)editor.GetType().GetAttribute(typeof(ResourceControllerAttribute));

                if (resourceControllerAttribute.PrototypeType == prototypeType)
                {
                    return editor;
                }
            }

            return null;
        }
    }
}
#endif