#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.Console;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.SelectionDatas;
using VladislavTsurikov.ShaderVariantCollection.Editor;

namespace VladislavTsurikov.RendererStack.Runtime.Core.RenderManager.RenderModes.GPUInstancedIndirect.GPUISupport
{
    public static class GPUInstancedIndirectShaderUtility 
    {
        public static void GenerateInstancedShadersIfNecessary(Prototype proto)
        {
            MeshRenderer[] meshRenderers = proto.GetMeshRenderers();

            List<string> brokenShaders = new List<string>();
            
            foreach (MeshRenderer mr in meshRenderers)
            {
                foreach (var material in mr.sharedMaterials)
                {
                    if (material == null || material.shader == null)
                    {
                        continue;
                    }

                    if (GPUInstancedIndirectShaderStack.Instance.IsShadersInstancedVersionExists(material.shader.name))
                    {
                        ShaderVariantCollectionUtility.AddShaderVariantToCollection(GPUInstancedIndirectShaderStack.Instance.GetShader(material.shader.name), material.shaderKeywords);
                        continue;
                    }

                    if (!Application.isPlaying)
                    {
                        if (IsShaderSupportInstancedIndirect(material.shader))
                        {
                            GPUInstancedIndirectShaderStack.Instance.AddShaderInstance(material.shader.name, material.shader, true);
                            ShaderVariantCollectionUtility.AddShaderVariantToCollection(GPUInstancedIndirectShaderStack.Instance.GetShader(material.shader.name), material.shaderKeywords);
                        }
                        else if(RendererStackSettings.Instance.AutoShaderConversion)
                        {
                            Shader shader = GenerateGPUInstancedIndirectShader.Create(material.shader);
                            if (shader != null)
                            {
                                GPUInstancedIndirectShaderStack.Instance.AddShaderInstance(material.shader.name, shader);
                                ShaderVariantCollectionUtility.AddShaderVariantToCollection(GPUInstancedIndirectShaderStack.Instance.GetShader(material.shader.name), material.shaderKeywords);
                            }
                            else 
                            {
                                if (!brokenShaders.Contains(material.shader.name))
                                {
                                    string warningShaderGraph = "{0} is created with ShaderGraph, and GPU Instanced Indirect Setup is not present.";

                                    string originalAssetPath = AssetDatabase.GetAssetPath(material.shader);
                                    if (originalAssetPath.ToLower().EndsWith(".shadergraph"))
                                    {
                                        PrototypeConsole.Log(proto, new PrototypeLog(string.Format(warningShaderGraph, material.shader.name), "Please copy the shader code and paste it to a new shader file. When you set this new shader to your material, Renderer can run auto-conversion on this shader."));
                                        brokenShaders.Add(material.shader.name);
                                    }
                                    else
                                    {
                                        PrototypeConsole.Log(proto, new PrototypeLog("Can not create instanced version for shader: " + material.shader.name, "Standard Shader will be used instead. If you are using a Unity built-in shader, please download the shader to your project from the Unity Archive."));
                                        brokenShaders.Add(material.shader.name);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static bool IsShaderSupportInstancedIndirect(Shader shader)
        {
            if (shader == null || shader.name == RendererStackConstants.ShaderUnityInternalError)
            {
                Debug.LogError("Can not find shader! Please make sure that the material has a shader assigned.");
                return false;
            }

            string originalShaderText = System.IO.File.ReadAllText(AssetDatabase.GetAssetPath(shader));

            if (!string.IsNullOrEmpty(originalShaderText))
            {
                return originalShaderText.Contains("GPUInstancedIndirectInclude.cginc");
            }
            return false;
        }
    }
}
#endif