﻿#if RENDERER_STACK
using System.Collections.Generic;
using UnityEngine;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject
{
    public static class AllAvailableTerrainObjectPrototypes
    {
        private static readonly Dictionary<int, List<PrototypeTerrainObject>> _prototypes = new();

        static AllAvailableTerrainObjectPrototypes()
        {
            if (Resources.FindObjectsOfTypeAll(typeof(Group)) is Group[] groups)
            {
                foreach (Group group in groups)
                {
                    if (group.PrototypeType != typeof(PrototypeTerrainObject))
                    {
                        continue;
                    }
                    
                    foreach (var prototype in group.PrototypeList)
                    {
                        AddPrototype((PrototypeTerrainObject)prototype);
                    }
                }
            }
        }

        public static void AddPrototype(PrototypeTerrainObject prototype)
        {
            if (!_prototypes.TryAdd(prototype.RendererPrototypeID, new List<PrototypeTerrainObject> { prototype }))
            {
                if(_prototypes.TryGetValue(prototype.RendererPrototypeID, out var prototypes))
                {
                    if (!prototypes.Contains(prototype))
                    {
                        prototypes.Add(prototype);
                    }
                }
            }
        }
        
        public static void RemovePrototype(PrototypeTerrainObject prototype)
        {
            if(_prototypes.TryGetValue(prototype.RendererPrototypeID, out var prototypes))
            {
                prototypes.Remove(prototype);

                if (prototypes.Count == 0)
                {
                    _prototypes.Remove(prototype.RendererPrototypeID);
                }
            }
        }
        
        public static List<PrototypeTerrainObject> GetPrototypes(int rendererPrototypeID)
        {
            return _prototypes.TryGetValue(rendererPrototypeID, out var prototypes) ? prototypes : null;
        }
    }
}
#endif