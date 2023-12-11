using System;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;

namespace VladislavTsurikov.IMGUIUtility.Editor
{
    [Serializable]
    public class IMGUIPath : ScriptableObject
    {
        private const string AssetsPath = "Assets/";

        public static string PathToBaseFolder = BasePath;

        private const string GUISkinFileName = "GUISkin.guiskin";
        public static string FoldoutRightFileName = "FoldoutRight.png";
        public static string FoldoutDownFileName = "FoldoutDown.png";
        
        public static string GUISkin = "GUISkin";
        public static string Images = "Images";
        public static string Foldout = "Foldout";

        public static string PathToGUISkin = CombinePath(PathToBaseFolder, GUISkin);
        public static string PathToImages = CombinePath(PathToGUISkin, Images);
        public static string PathToFoldout = CombinePath(PathToImages, Foldout);

        public static string FoldoutRightPath = CombinePath(PathToFoldout, FoldoutRightFileName);
    	public static string FoldoutDownPath = CombinePath(PathToFoldout, FoldoutDownFileName);

        public static readonly string SkinPath = CombinePath(PathToGUISkin, GUISkinFileName);

        private static string _sBasePath;

        public static string BasePath
        { 
            get
            {
#if UNITY_EDITOR

                if (!string.IsNullOrEmpty(_sBasePath)) return _sBasePath;
                var obj = CreateInstance<IMGUIPath>();
                UnityEditor.MonoScript s = UnityEditor.MonoScript.FromScriptableObject(obj);
                string assetPath = UnityEditor.AssetDatabase.GetAssetPath(s);
                DestroyImmediate(obj);
                var fileInfo = new FileInfo(assetPath);
                Debug.Assert(fileInfo.Directory != null, "fileInfo.Directory != null");
                Debug.Assert(fileInfo.Directory.Parent != null, "fileInfo.Directory.Parent != null");
                DirectoryInfo baseDir = fileInfo.Directory.Parent;
                Debug.Assert(baseDir != null, "baseDir != null");
                Assert.AreEqual("IMGUIUtility", baseDir.Name);
                string baseDirPath = ReplaceBackslashesWithForwardSlashes(baseDir.ToString());
                int index = baseDirPath.LastIndexOf(AssetsPath, StringComparison.Ordinal);
                Assert.IsTrue(index >= 0);
                baseDirPath = baseDirPath.Substring(index);
                _sBasePath = baseDirPath;

                PathToBaseFolder = _sBasePath;

                return _sBasePath;

#else
                return "";
#endif
            }
        }

        public static string ReplaceBackslashesWithForwardSlashes(string path)
        {
            return path.Replace('\\', '/');
        }

        public static string CombinePath(string path1, string path2)
        {
            return path1 + "/" + path2;
        }
    }
}