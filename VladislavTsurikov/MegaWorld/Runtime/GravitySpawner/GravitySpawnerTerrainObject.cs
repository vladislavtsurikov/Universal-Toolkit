using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime.Scene;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.PhysicsToolsSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Stamper;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using PrototypeTerrainObject = VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.PrototypeTerrainObject;

namespace VladislavTsurikov.MegaWorld.Runtime.GravitySpawner
{
    public class GravitySpawnerTerrainObject : TerrainObjectOnDisablePhysics
    {
        private readonly TerrainsMaskManager _terrainsMaskManager;
        private readonly BoxArea _area;
        
        public GravitySpawnerTerrainObject(Group group, PrototypeTerrainObject proto, TerrainsMaskManager terrainsMaskManager, BoxArea area) : base(group, proto)
        {
            _terrainsMaskManager = terrainsMaskManager;
            _area = area;
        }
        
        protected override float GetFitness(RayHit rayHit)
        {
            return Utility.GetFitness.Get(_group, _terrainsMaskManager, rayHit);
        }
        
        protected override bool IsValid()
        {
            if (!_area.Contains(SimulatedBody.GameObject.transform.position))
            {
                Object.DestroyImmediate(SimulatedBody.GameObject);
                return false;
            }

            return true;
        }
    }
}