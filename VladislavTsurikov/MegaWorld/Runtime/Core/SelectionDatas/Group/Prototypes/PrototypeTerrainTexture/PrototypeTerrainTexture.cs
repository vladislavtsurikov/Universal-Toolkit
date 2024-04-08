﻿using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.Core.Runtime.IconStack.Attributes;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainTexture;
#endif

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainTexture
{
    [Serializable]
    [MenuItem("Unity/Terrain Texture")]
    [MissingIconsWarning("Drag & Drop Texture or Terrain Layers Here")]
    [DropObjects(new[]{typeof(Texture2D), typeof(TerrainLayer)})]
    public class PrototypeTerrainTexture : Prototype
    {
        public TerrainLayer TerrainLayer;
        
        public override string Name
        {
	        get
	        {
		        if (TerrainLayer != null || TerrainLayer.diffuseTexture != null)
		        {
			        return TerrainLayer.name;
		        }

		        return "Missing Texture";
	        }
        }
        
        public override Object PrototypeObject => TerrainLayer;

#if UNITY_EDITOR
        public override Texture2D PreviewTexture
        {
	        get
	        {
		        if (TerrainLayer.diffuseTexture != null)
		        {
			        return TerrainLayer.diffuseTexture;  
		        }

		        return null;
	        }
        }
#endif

        public override void OnCreatePrototype(Object obj)
	    {
		    if (obj is Texture2D texture2D)
		    {
			    TerrainLayer = Convert(texture2D);
		    }
		    else
		    {
			    TerrainLayer = (TerrainLayer)obj;
		    }
	    }

        public override bool IsSamePrototypeObject(Object obj)
	    {
		    return PrototypeObject == obj;
	    }

	    public TerrainLayer Convert(Texture2D diffuseTexture)
	    {
		    TerrainLayer terrainLayer = new TerrainLayer
		    {
			    metallic = 0.0f,
			    normalMapTexture = null,
			    smoothness = 0.0f,
			    diffuseTexture = diffuseTexture,
			    tileOffset = Vector2.zero,
			    tileSize = Vector2.one,
			    specular = Color.black
		    };
		    
#if UNITY_EDITOR
		    return ProfileFactory.SaveTerrainLayerAsAsset(diffuseTexture.name, terrainLayer);
#else
			return terrainLayer;
#endif
	    }
    }
}