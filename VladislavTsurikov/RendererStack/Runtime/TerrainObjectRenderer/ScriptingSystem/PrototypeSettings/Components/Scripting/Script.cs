using System;
using UnityEngine;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data;
using Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.ScriptingSystem.PrototypeSettings.Scripting
{
    [Serializable]
    public abstract class Script : Component
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
