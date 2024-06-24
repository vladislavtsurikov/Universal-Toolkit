using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.PrototypeSettings;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.ScriptingSystem.PrototypeSettings.Scripting;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.ScriptingSystem.MonoBehaviour
{
    public class HierarchyTerrainObjectInstance : UnityEngine.MonoBehaviour
    {
        public TerrainObjectInstance TerrainObjectInstance { get; private set; }
        
        //What objects cause this GameObject to appear
        internal readonly List<object> UsedObjects = new List<object>();
        
        internal void Setup(TerrainObjectInstance instance, Scripting scriptingComponent)
        {
            TerrainObjectInstance = instance;
            instance.HierarchyTerrainObjectInstance = this;
			
            if (PrototypeComponent.IsValid(scriptingComponent))
            {
                if (!Application.isPlaying)
                {
                    return;
                }

                foreach (var script in scriptingComponent.ScriptStack.ElementList)
                {
                    instance.AddScript(script);
                }
				
                instance.SetupScripts();
            }
        }

        private void Update()
        {
            foreach (var script in TerrainObjectInstance.GetScripts())
            {
                script.Update();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            foreach (var script in TerrainObjectInstance.GetScripts())
            {
                script.OnCollisionEnter(collision);
            }
        }
    }
}