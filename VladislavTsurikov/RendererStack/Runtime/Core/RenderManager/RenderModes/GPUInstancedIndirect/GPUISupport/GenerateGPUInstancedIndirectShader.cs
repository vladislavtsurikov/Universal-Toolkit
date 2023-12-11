#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.Core.Runtime;

namespace VladislavTsurikov.RendererStack.Runtime.Core.RenderManager.RenderModes.GPUInstancedIndirect.GPUISupport
{
    public static class GenerateGPUInstancedIndirectShader 
    {
        [NonSerialized] 
        private static string[] _gpuInstancedIndirectSetup = new string[]
        {
            "#include \"XXX\"\n",
            "#pragma instancing_options procedural:setupGPUInstancedIndirect\n",
            "#pragma multi_compile_instancing\n",
        }; 

        static GenerateGPUInstancedIndirectShader()
        {
            string[] searchFolders = AssetDatabase.FindAssets("GPUInstancedIndirectInclude");

            for (int i = 0; i < searchFolders.Length; i++)
            {
                if (AssetDatabase.GUIDToAssetPath(searchFolders[i]).EndsWith("GPUInstancedIndirectInclude.cginc")) 
                {
                    string cgincQr = AssetDatabase.GUIDToAssetPath(searchFolders[i]);

                    _gpuInstancedIndirectSetup[0] = _gpuInstancedIndirectSetup[0].Replace("XXX", cgincQr);

                    return;
                }
            }
        }

        private static List<string> RemoveExistingProceduralSetup(string originalAssetPath)
        {
            string[] originalLines = System.IO.File.ReadAllLines(originalAssetPath);
            List<string> newShaderlines = new List<string>();

            foreach (string line in originalLines)
            {
                if (!line.Contains("#pragma instancing_options")
                    && !line.Contains("GPUInstancedIndirectInclude.cginc")
                    && !line.Contains("#pragma multi_compile_instancing"))
                {
                    newShaderlines.Add(line + "\n");
                }
            }

            return newShaderlines; 
        }

        private static List<string> ChangeShaderName(string newShaderName, Shader originalShader, List<string> shaderLines)
        { 
            for (int i = 0; i < shaderLines.Count; i++)
            {
                if (shaderLines[i].Contains(originalShader.name))
                {
                    shaderLines[i] = shaderLines[i].Replace(originalShader.name, newShaderName);
                } 
            }

            return shaderLines;
        }

        private static List<string> AddGPUInstancedIndirectSupport(List<string> shaderLines)
        {
            for (int lineIndex = 0; lineIndex < shaderLines.Count; lineIndex++)
            {
                if (shaderLines[lineIndex].Contains("HLSLPROGRAM"))
                {
                    for (int i = lineIndex; i < shaderLines.Count; i++)
                    {
                        if (shaderLines[i].Contains("ENDHLSL"))
                        {
                            shaderLines.InsertRange(i, _gpuInstancedIndirectSetup);
                            break;
                        }
                    }
                }

                if(shaderLines[lineIndex].Contains("CGPROGRAM"))
                {
                    for (int i = lineIndex; i < shaderLines.Count; i++)
                    {
                        if (shaderLines[i].Contains("ENDCG"))
                        {
                            shaderLines.InsertRange(i, _gpuInstancedIndirectSetup);
                            break;
                        }
                    }
                }
            }
            
            return shaderLines;
        }

        public static Shader Create(Shader originalShader, bool useOriginal = false)
        { 
            if (originalShader == null || originalShader.name == RendererStackConstants.ShaderUnityInternalError)
            {
                Debug.LogError("Can not find shader! Please make sure that the material has a shader assigned.");
                return null;
            }

            Shader originalShaderRef = Shader.Find(originalShader.name);
            string originalAssetPath = AssetDatabase.GetAssetPath(originalShaderRef);

            // can not work with ShaderGraph or other non shader code
            if (!originalAssetPath.EndsWith(".shader"))
            {   
                return null;
            }

            List<string> shaderLines = new List<string>();

            shaderLines = RemoveExistingProceduralSetup(originalAssetPath);
            
            EditorUtility.DisplayProgressBar("Shader Conversion", "Creating a shader with GPU Instanced Instanced support...", 0.1f);

            string newShaderName = useOriginal ? "" : "GPUInstancedIndirect/" + originalShader.name;
            shaderLines = ChangeShaderName(newShaderName, originalShader, shaderLines);
            shaderLines = AddGPUInstancedIndirectSupport(shaderLines);
            
            Shader shader = CreateShader(originalAssetPath, shaderLines, useOriginal);
            
            EditorUtility.ClearProgressBar();

            return shader;
        }

        private static Shader CreateShader(List<string> shaderLines, string assetPath)
        {
            byte[] bytes = GetBytesFromStrings(shaderLines);
            System.IO.FileStream fs = System.IO.File.Create(assetPath);
            fs.Write(bytes, 0, bytes.Length);
            fs.Close();

            EditorUtility.DisplayProgressBar("Shader Conversion", "Creating a shader with GPU Instanced Instanced support...", 0.3f);
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
            AssetDatabase.Refresh();

            Shader shader = AssetDatabase.LoadAssetAtPath<Shader>(assetPath);

            return shader;
        }

        private static Shader CreateShader(string originalAssetPath, List<string> shaderLines, bool useOriginal)
        {
            string originalFileName = System.IO.Path.GetFileName(originalAssetPath);
            
            if (originalAssetPath.StartsWith("Packages/"))
            {
                if (!System.IO.Directory.Exists(RendererStackConstants.PathToShaders))
                {
                    System.IO.Directory.CreateDirectory(RendererStackConstants.PathToShaders);
                }
                
                string assetPath = CommonPath.CombinePath(RendererStackConstants.PathToShaders, originalFileName);

                string newAssetPath = assetPath.Replace(originalFileName, originalFileName.Replace(".shader", "_GPUInstancedIndirect.shader"));

                return CreateShader(shaderLines, newAssetPath);
            }
            else
            {
                string newAssetPath = useOriginal ? originalAssetPath : originalAssetPath.Replace(originalFileName, originalFileName.Replace(".shader", "_GPUInstancedIndirect.shader"));

                return CreateShader(shaderLines, newAssetPath);
            }
        }
        
        public static byte[] GetBytesFromStrings(List<string> shaderLines)
        {
            string shaderText = "";
            foreach (string line in shaderLines)
            {
                shaderText += line;
            }

            return System.Text.Encoding.UTF8.GetBytes(shaderText);
        }
    }
}
#endif