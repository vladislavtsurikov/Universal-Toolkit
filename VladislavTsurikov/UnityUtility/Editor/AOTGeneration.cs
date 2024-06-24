#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.Core.Runtime;
using VladislavTsurikov.OdinSerializer.Unity_Integration;

namespace VladislavTsurikov.UnityUtility.Editor
{
    public static class AOTGeneration
    {
        [MenuItem("Tools/Vladislav Tsurikov/Generate AOT DLL", false, 0)]
        private static void OpenMegaWorldWindow()
        {
            AOTSupportUtilities.ScanProjectForSerializedTypes(out var serializedTypes);

            Debug.Log(serializedTypes.Count);

            string pathToFolder = CommonPath.CombinePath(CommonPath.PathToResources, "AOT Generation");

            Directory.CreateDirectory(pathToFolder);
                    
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            AOTSupportUtilities.GenerateDLL(pathToFolder, "VladislavTsurikov", serializedTypes);
        }
    }
}
#endif