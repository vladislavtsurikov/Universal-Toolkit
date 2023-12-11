using System;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.Core.Runtime.IconStack.Attributes;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.RenderModelData.Utility;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.SelectionDatas;
using GameObjectUtility = VladislavTsurikov.Utility.Runtime.GameObjectUtility;
using MeshUtility = VladislavTsurikov.Utility.Runtime.MeshUtility;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using GUIUtility = VladislavTsurikov.Utility.Runtime.GUIUtility;
#endif

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer
{
    [Serializable]
    [MissingIconsWarning("Drag & Drop Prefabs Here")]
    [DropObjects(new[]{typeof(GameObject)})]
    public class PrototypeTerrainObject : Prototype
    {
        public GameObject Prefab;
        public Bounds Bounds;

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
        public override LayerMask Layer => Prefab.layer;

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

        protected override void InitPrototype(Object obj)
        {
            Prefab = (GameObject)obj;
            Bounds = MeshUtility.CalculateBoundsInstantiate(Prefab);      
        }

        public override MeshRenderer[] GetMeshRenderers()
        {
            return Prefab.GetComponentsInChildren<MeshRenderer>();
        }

        public override void RefreshRenderModelInfo()
        {
            RenderModel = RenderModelUtility.GetRenderModel(this, Prefab);
        }

        public override bool IsSamePrototypeObject(Object obj)
        {
            if (obj == null)
            {
                return false;
            }
            
            GameObject go = (GameObject)obj;
            
#if UNITY_EDITOR
            if(PrefabUtility.GetPrefabAssetType(obj) == PrefabAssetType.NotAPrefab)
            {
                return false;
            }
#endif
            
            return GameObjectUtility.IsSameGameObject(go, Prefab);
        }
    }
}