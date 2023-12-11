using System;
using UnityEngine;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.RendererData;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.ScriptingSystem.PrototypeSettings.Components.Scripting
{
    [Serializable]
    public abstract class Script : ComponentStack.Runtime.Component
    {
        [NonSerialized]
        protected TerrainObjectInstance TerrainObjectInstance;

        protected override void SetupElement(object[] args = null)
        {
            if (args != null && args.Length != 0)
            {
                TerrainObjectInstance = (TerrainObjectInstance)args[0];
            }
            
            OnEnable();
        }
        
        public virtual void OnEnable() { }
        public virtual void Update() { }
        public virtual void OnCollisionEnter(Collision collision) { }
    }
}
