using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;

namespace VladislavTsurikov.RendererStack.Runtime.Core.RendererSystem
{
    [Serializable]
    public class RendererStack : ComponentStackOnlyDifferentTypes<CustomRenderer>
    {
        public void CheckChanges()
        {
            if (!Application.isPlaying)
            {
                foreach (CustomRenderer setting in _elementList)
                {
                    setting?.CheckChanges(); 
                }
            }
        }
        
        public void ForceUpdateRendererData()
        {
            foreach (var customRenderer in _elementList)
            {
                customRenderer.ForceUpdateRendererData = true;
            }
        }

        public void Render()
        {
            if(!RendererStackManager.Instance.IsSetup)
            {
                return;
            }

            foreach (var setting in _elementList)
            {
                if (!setting.IsSetup)
                {
                    continue;
                }

                if (setting.Active)
                {
                    setting.Render();
                }
            }
        }
    }
}