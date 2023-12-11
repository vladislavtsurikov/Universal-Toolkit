#if UNITY_EDITOR 
using UnityEngine;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.API;

namespace VladislavTsurikov.PhysicsSimulatorEditor.Editor.Integrations.TerrainObjectRenderer
{
    public class SimulatedTerrainObjectBody : SimulatedBody
    {
        private PrototypeTerrainObject _proto;
        
        public SimulatedTerrainObjectBody(PrototypeTerrainObject proto, GameObject gameObject) : base(gameObject)
        {
            _proto = proto;
            
            OnDisablePhysicsSupport += AddInstance;

            gameObject.hideFlags = HideFlags.HideInHierarchy;
        }

        private void AddInstance()
        {
            TerrainObjectRendererAPI.AddInstance(_proto, _gameObject.transform.position, _gameObject.transform.lossyScale, _gameObject.transform.rotation);
            Object.DestroyImmediate(_gameObject);
        }
    }
}
#endif