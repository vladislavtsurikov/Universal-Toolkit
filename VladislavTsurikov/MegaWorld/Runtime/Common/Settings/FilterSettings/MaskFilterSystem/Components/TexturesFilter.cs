#if UNITY_EDITOR
#endif
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.TerrainTools;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Utility;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainTexture;
using VladislavTsurikov.Utility.Runtime;

#if UNITY_2021_2_OR_NEWER
#else
using UnityEngine.Experimental.TerrainAPI;
#endif

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Components 
{
    [Serializable]
    [MenuItem("Textures")]
    public class TexturesFilter : MaskFilter 
    {
        public BlendMode BlendMode = BlendMode.Multiply;
        public List<TerrainTexture> TerrainTextureList = new List<TerrainTexture>();
        
        [OnDeserializing]
        public void Init()
        {
            if (TerrainTextureList == null)
            {
                TerrainTextureList = new List<TerrainTexture>();
            }
        }

        public override void Eval(MaskFilterContext fc, int index) 
        {
            if (Terrain.activeTerrain == null)
            {
                return;
            }
            
            Terrain terrain = fc.BoxArea.TerrainUnder;

            Vector2 currUV = UnityTerrainUtility.WorldPointToUV(fc.BoxArea.Center, terrain);

            BrushTransform brushTransform = TerrainPaintUtility.CalculateBrushTransform(terrain, currUV, fc.BoxArea.BoxSize, 0.0f);
            Rect brushRect = brushTransform.GetBrushXYBounds();

            List<TerrainTexture> addTexturesToRenderTextureList = new List<TerrainTexture>();

            if(IsSyncTerrain(Terrain.activeTerrain) == false)
			{
				UpdateCheckTerrainTextures(Terrain.activeTerrain);
			}

            for (int i = 0; i < TerrainTextureList.Count; i++)
            {
                if(TerrainTextureList[i].Selected)
                {
                    addTexturesToRenderTextureList.Add(TerrainTextureList[i]);
                }
            }

            Material blendMat = MaskFilterUtility.blendModesMaterial;

            RenderTexture output = RenderTexture.GetTemporary(fc.SourceRenderTexture.width, fc.SourceRenderTexture.height, fc.SourceRenderTexture.depth, GraphicsFormat.R16_SFloat);
            output.enableRandomWrite = true;

            foreach (var terrainTexture in addTexturesToRenderTextureList)
            {
                RenderTexture localSourceRender = RenderTexture.GetTemporary(fc.SourceRenderTexture.width, fc.SourceRenderTexture.height, 1, RenderTextureFormat.ARGB32);
                localSourceRender.enableRandomWrite = true;

                PaintContext localTextureContext = TerrainPaintUtility.BeginPaintTexture(terrain, brushRect, Terrain.activeTerrain.terrainData.terrainLayers[terrainTexture.TerrainProtoId]);

                blendMat.SetInt("_BlendMode", (int)BlendMode.Add);
                blendMat.SetTexture("_MainTex", output);
                blendMat.SetTexture("_BlendTex", localTextureContext.sourceRenderTexture);

                Graphics.Blit(output, localSourceRender, blendMat, 0);
                Graphics.Blit(localSourceRender, output);

                TerrainPaintUtility.ReleaseContextResources(localTextureContext); 

                RenderTexture.ReleaseTemporary(localSourceRender);
            }

            RenderTexture sourceRender = RenderTexture.GetTemporary(fc.SourceRenderTexture.width, fc.SourceRenderTexture.height, 1, RenderTextureFormat.ARGB32);
            sourceRender.enableRandomWrite = true;

            if(index == 0)
            {
                blendMat.SetInt("_BlendMode", (int)BlendMode.Multiply);
            }
            else
            {
                blendMat.SetInt("_BlendMode", (int)BlendMode);
            }

            blendMat.SetTexture("_MainTex", output);
            blendMat.SetTexture("_BlendTex", fc.SourceRenderTexture);

            Graphics.Blit(output, fc.DestinationRenderTexture, blendMat, 0);

            RenderTexture.ReleaseTemporary(output);
            RenderTexture.ReleaseTemporary(sourceRender);
        }

        public bool IsSyncTerrain(Terrain terrain)
        {
            foreach (var layer in terrain.terrainData.terrainLayers)
            {
                if (layer == null)
                {
                    continue;
                }
                
                bool find = false;

                for (int i = 0; i < TerrainTextureList.Count; i++)
                {
                    if (TextureUtility.IsSameTexture(layer.diffuseTexture, TerrainTextureList[i].Texture, false))
                    {
                        find = true;
                        break;
                    }
                }

                if(find == false)
                {
                    return false;
                }
            }

            return true;
        }

        public void UpdateCheckTerrainTextures(Terrain activeTerrain)
        {
            if (activeTerrain == null)
            {
                Debug.LogWarning("Can not update prototypes from the terrain as there is no terrain currently active in this scene.");
                return;
            }

            int idx;

            TerrainTextureList.Clear();
            
            TerrainLayer[] terrainLayers = activeTerrain.terrainData.terrainLayers;

            for (idx = 0; idx < terrainLayers.Length; idx++)
            {
                if (terrainLayers[idx] == null)
                {
                    continue;
                }
                
                var checkTerrainTextures = new TerrainTexture
                {
                    Texture = terrainLayers[idx].diffuseTexture,
                    TerrainProtoId = idx
                };

                TerrainTextureList.Add(checkTerrainTextures);
            }
        }
    }
}