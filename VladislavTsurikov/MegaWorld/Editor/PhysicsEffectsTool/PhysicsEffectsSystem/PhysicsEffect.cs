#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime.Scene;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.Utility;
using VladislavTsurikov.PhysicsSimulatorEditor.Editor;
using VladislavTsurikov.Utility.Runtime;

namespace VladislavTsurikov.MegaWorld.Editor.PhysicsEffectsTool.PhysicsEffectsSystem
{
    public abstract class PhysicsEffect : ComponentStack.Runtime.Component
    {
        public float Size = 10.0F;
        public float PositionOffsetY = 0;
        
        public void DoEffect(RayHit rayHit)
        {
            List<GameObject> prefabList = new List<GameObject>();

            GetGameObjectFromPhysicsOverlapSphere(rayHit, prefabRoot =>
            {
                prefabList.Add(prefabRoot);

                ApplyEffectIfNecessary(prefabRoot, GetPositionOffsetY(rayHit));

                return true;
            });

            GetGameObject(rayHit, prefabRoot =>
            {
                if(!prefabList.Contains(prefabRoot))
                {
                    ApplyEffectIfNecessary(prefabRoot, GetPositionOffsetY(rayHit));
                }

                return true;
            }); 

            PhysicsSimulator.Activate(DisablePhysicsMode.GlobalTime, false, true);
            ActiveTimePhysicsSimulator.RefreshTime(); 
        }

        private void ApplyEffectIfNecessary(GameObject prefabRoot, Vector3 positionOffsetY)
        {
            List<PrototypeGameObject> prototypes = GetPrototypeUtility.GetPrototypes<PrototypeGameObject>(prefabRoot);

            foreach (var proto in prototypes)
            {
                if(proto != null && proto.Selected)
                {
                    float distanceFromObject = Vector3.Distance(positionOffsetY, prefabRoot.transform.position);

                    if(distanceFromObject < Size / 2)
                    {
                        SimulatedBody simulatedBody = GetSimulatedBody(prefabRoot);
                        if(simulatedBody.Rigidbody != null)
                        {
                            ApplyEffect(positionOffsetY, simulatedBody.Rigidbody);
                        }
                    }
                }
            }
        }

        private SimulatedBody GetSimulatedBody(GameObject prefabRoot)
        {
            if(SimulatedBodyStack.GetSimulatedBody(prefabRoot) == null)
            {
                SimulatedBody simulatedBody = new SimulatedBody(prefabRoot);
                PhysicsSimulator.RegisterGameObject(simulatedBody);

                return simulatedBody;
            }
            else
            {
                SimulatedBody simulatedBody = SimulatedBodyStack.GetSimulatedBody(prefabRoot);
                return simulatedBody;
            }
        }

        private void GetGameObject(RayHit rayHit, Func<GameObject, bool> func)
        {
            Bounds bounds = new Bounds(rayHit.Point, new Vector3(Size, Size, Size));

            PrototypeGameObjectOverlap.OverlapBox(bounds, (proto, go) =>
            {
                if(proto == null || proto.Active == false || proto.Selected == false)
                {
                    return true;
                }

                GameObject prefabRoot = GameObjectUtility.GetPrefabRoot(go);
                if (prefabRoot == null)
                {
                    return true;
                }

                func.Invoke(prefabRoot);

                return true;
            });
        }

        private void GetGameObjectFromPhysicsOverlapSphere(RayHit rayHit, Func<GameObject, bool> func)
        {
            Collider[] colliders = Physics.OverlapSphere(GetPositionOffsetY(rayHit), Size / 2);
            foreach (Collider hit in colliders)
            {
                GameObject prefabRoot = GameObjectUtility.GetPrefabRoot(hit.gameObject);
                if (prefabRoot == null)
                {
                    continue;
                }

                func.Invoke(prefabRoot);
            }
        }

        protected Vector3 GetPositionOffsetY(RayHit rayHit)
        {
            Vector3 positionOffsetY = rayHit.Point;
            positionOffsetY.y += PositionOffsetY;

            return positionOffsetY;
        }

        protected float GetOpacity(float force, float maxForce)
        {
            float difference = force / maxForce;
            difference = Mathf.InverseLerp(0, 0.7f, difference);

            difference = difference < 0.3 ?  0.3f : difference;

            return difference * 0.6f;
        }

        public virtual void ApplyEffect(Vector3 positionOffsetY, Rigidbody rb){}
        public virtual void OnRepaint(RayHit rayHit){}
	}
}
#endif