using System.Collections;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime.Scene;
using VladislavTsurikov.Coroutines.Runtime;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.Utility;
using VladislavTsurikov.PhysicsSimulator.Runtime.SimulatedBody;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.API;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.RendererData;
using VladislavTsurikov.Utility.Runtime.Extensions;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.PhysXPainter
{
    public class TerrainObjectOnDisablePhysics : OnDisableSimulatedBodyAction
    {
        protected readonly Group _group;
        protected readonly PrototypeTerrainObject _proto;
        
        public TerrainObjectOnDisablePhysics(Group group, PrototypeTerrainObject proto)
        {
            _group = group;
            _proto = proto;
        }

        protected override void OnDisablePhysics()
        {
            if (!IsValid())
            {
                return;
            }
            
            var position = SimulatedBody.GameObject.transform.position;
            
            bool addedInstance = false;

            RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayDown(new Vector3(position.x, position.y + 1, position.z)), 
                GlobalCommonComponentSingleton<LayerSettings>.Instance.GetCurrentPaintLayers(_group.PrototypeType));
            
            if (rayHit != null)
            {
                float fitness = GetFitness(rayHit);
                
                if (fitness != 0)
                {
                    if (Random.Range(0f, 1f) < 1 - fitness)
                    {
                        Object.DestroyImmediate(SimulatedBody.GameObject);
                        return;
                    }
                    
                    TerrainObjectInstance terrainObjectInstance = TerrainObjectRendererAPI.AddInstance(_proto,
                        SimulatedBody.GameObject.transform.position, SimulatedBody.GameObject.transform.lossyScale,
                        SimulatedBody.GameObject.transform.rotation);

                    TerrainObjectSimulatedBody terrainObjectSimulatedBody = (TerrainObjectSimulatedBody)SimulatedBody;
                    terrainObjectSimulatedBody.TerrainObjectInstance = terrainObjectInstance;

                    addedInstance = true;
                }
            }

            if (addedInstance)
            {
                SimulatedBody.GameObject.DisableMeshRenderers();

                CoroutineRunner.StartCoroutine(DestroyGameObject());
                
                IEnumerator DestroyGameObject()
                {
                    yield return new YieldCustom(() => SimulatedBodyStack.Count == 0);
                    
                    Object.DestroyImmediate(SimulatedBody.GameObject);
                }
            }
            else
            {
                Object.DestroyImmediate(SimulatedBody.GameObject);
            }
        }

        protected virtual float GetFitness(RayHit rayHit)
        {
            return 1;
        }
        
        protected virtual bool IsValid()
        {
            return true;
        }
    }
}