using System;
using UnityEngine;
using VladislavTsurikov.MegaWorld.Runtime.Common;
using VladislavTsurikov.Utility.Runtime;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using GUIUtility = VladislavTsurikov.Utility.Runtime.GUIUtility;
#endif

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes
{
    public abstract class PlacedObjectPrototype : Prototype
    {
        public GameObject Prefab;
        
        [NonSerialized] 
        public PastTransform PastTransform;
        public Vector3 Extents = Vector3.one;

        public override string Name
        {
            get
            {
                if (Prefab != null)
                {
                    return Prefab.name;
                }

                return "Missing Prefab";
            }
        }

        public override Object PrototypeObject => Prefab;

#if UNITY_EDITOR
        public override Texture2D PreviewTexture
        {
            get
            {
                if (Prefab != null)
                {
                    return GUIUtility.GetPrefabPreviewTexture(Prefab);  
                }

                return null;
            }
        }
#endif

        public override void Init(Object obj)
        {
            Prefab = (GameObject)obj;
            PastTransform = new PastTransform(Prefab.transform);
            Extents = MeshUtility.CalculateBoundsInstantiate(Prefab).extents;    
        }
    }
}