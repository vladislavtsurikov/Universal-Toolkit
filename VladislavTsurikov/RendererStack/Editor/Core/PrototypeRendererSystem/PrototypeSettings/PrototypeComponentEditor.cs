#if UNITY_EDITOR
using System;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.PrototypeSettings;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.SelectionDatas;

namespace VladislavTsurikov.RendererStack.Editor.Core.PrototypeRendererSystem.PrototypeSettings
{
    public class PrototypeComponentEditor : ReorderableListComponentEditor
    {
        private Type _rendererType;
        
        protected Prototype Prototype => TargetPrototypeComponent.Prototype;

        private PrototypeComponent TargetPrototypeComponent => (PrototypeComponent)Target;

        protected Type RendererType
        {
            get
            {
                if (_rendererType == null)
                {
                    foreach (var prototypeRendererStack in PrototypesStorage.Instance.PrototypeRendererStacks)
                    {
                        if (prototypeRendererStack.GetPrototype(Prototype.ID) != null)
                        {
                            _rendererType = prototypeRendererStack.RendererType;
                        }
                    }
                }

                return _rendererType;
            }
        }
    }
}
#endif