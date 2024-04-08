using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.PhysicsSimulator.Runtime.SimulatedBody;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.RendererData;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.PhysXPainter
{
    public class TerrainObjectSimulatedBody : SimulatedBody
    {
        internal TerrainObjectInstance TerrainObjectInstance;
        
        public TerrainObjectSimulatedBody(GameObject gameObject) : base(gameObject)
        {
        }

        public TerrainObjectSimulatedBody(GameObject gameObject, List<OnDisableSimulatedBodyAction> onDisablePhysicsActions) : base(gameObject, onDisablePhysicsActions)
        {
        }
    }
}