using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.OverlapCheckSettings.OverlapChecks;
using VladislavTsurikov.MegaWorld.Runtime.Core.PreferencesSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.UnityUtility.Runtime;
using Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;
using Core_Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;
using Runtime_Core_Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.OverlapCheckSettings
{
    public enum OverlapShapeEnum 
    { 
        None, 
        OBB,
        Sphere,
    }
    
    [Name("Overlap Check Settings")]
    public class OverlapCheckSettings : Runtime_Core_Component
    {
        public OverlapShapeEnum OverlapShapeEnum = OverlapShapeEnum.Sphere;
        public OBBCheck ObbCheck = new OBBCheck();
        public SphereCheck SphereCheck = new SphereCheck();
        public CollisionCheck CollisionCheck = new CollisionCheck();

        public OverlapShape CurrentOverlapShape
        {
            get
            {
                return OverlapShapeEnum switch
                {
                    OverlapShapeEnum.OBB => ObbCheck,
                    OverlapShapeEnum.Sphere => SphereCheck,
                    _ => null
                };
            }
        }

        public static bool RunOverlapCheck(Type prototypeType, OverlapCheckSettings overlapCheckSettings, Vector3 extents, Instance instance)
        {
            if(overlapCheckSettings.CollisionCheck.RunCollisionCheck(extents, instance))
            {
                return false;
            }

            if(overlapCheckSettings.OverlapShapeEnum == OverlapShapeEnum.None)
            {
                return true;
            }

            if(prototypeType == typeof(PrototypeGameObject))
            {
#if UNITY_EDITOR
                if(!RunOverlapCheckForGameObject(new OverlapInstance(overlapCheckSettings, extents, instance)))
                {
                    return true;
                }
#endif
            }
#if RENDERER_STACK
            else 
            {
                if(!RunOverlapCheckForTerrainObject(new OverlapInstance(overlapCheckSettings, extents, instance)))
                {
                    return true;
                }
            }
#endif
            return false;
        }

#if RENDERER_STACK
        private static bool RunOverlapCheckForTerrainObject(OverlapInstance spawnOverlapInstance)
        {
            bool overlaps = false;

            PrototypeTerrainObjectOverlap.OverlapBox(spawnOverlapInstance.Obb.Center, spawnOverlapInstance.Obb.Size * PreferenceElementSingleton<OverlapCheckSettingsPreference>.Instance.MultiplyFindSize, 
                spawnOverlapInstance.Obb.Rotation, null, true,
                false, (proto, persistentInstance) =>
                {
                    OverlapCheckSettings localOverlapCheckSettings =
                        (OverlapCheckSettings)proto.GetElement(typeof(OverlapCheckSettings));

                    if (localOverlapCheckSettings.OverlapShapeEnum == OverlapShapeEnum.None)
                    {
                        return true;
                    }

                    OverlapInstance overlapInstance = new OverlapInstance(localOverlapCheckSettings,
                        persistentInstance.Position, persistentInstance.Scale, persistentInstance.Rotation,
                        proto.Extents);

                    if (overlapInstance.Intersects(spawnOverlapInstance))
                    {
                        overlaps = true;
                        return false;
                    }

                    return true;
                });

            return overlaps;
        }
#endif

#if UNITY_EDITOR
        private static bool RunOverlapCheckForGameObject(OverlapInstance spawnOverlapInstance)
        {
            bool overlaps = false;

            PrototypeGameObjectOverlap.OverlapBox(spawnOverlapInstance.Obb.Center, spawnOverlapInstance.Obb.Size * PreferenceElementSingleton<OverlapCheckSettingsPreference>.Instance.MultiplyFindSize, 
                spawnOverlapInstance.Obb.Rotation, (proto, go) =>
            {
                OverlapCheckSettings localOverlapCheckSettings = (OverlapCheckSettings)proto.GetElement(typeof(OverlapCheckSettings));

                if(localOverlapCheckSettings.OverlapShapeEnum == OverlapShapeEnum.None)
                {
                    return true;
                }
                
                OverlapInstance overlapInstance = new OverlapInstance(localOverlapCheckSettings,
                    go.transform.position, go.transform.lossyScale, go.transform.rotation, proto.Extents);

                if (overlapInstance.Intersects(spawnOverlapInstance))
                {
                    overlaps = true;
                    return false;
                }

                return true;
            });
            
            return overlaps;
        }
#endif
    }
}