#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

namespace VladislavTsurikov.SceneManagerTool.Editor
{
    public static class PackageInitializer
    {
        private static ListRequest _listPackageRequest;
        private static AddRequest _addPackageRequest;

        private static bool _isAddressablesInstalled;
        
        public static string ADDRESSABLES_KW = "SCENE_MANAGER_ADDRESSABLES";

        [DidReloadScripts]
        public static void Init()
        {
            CheckUnityPackagesAndInit();
        }

        private static void CheckUnityPackagesAndInit()
        {
            _isAddressablesInstalled = false;

            _listPackageRequest = Client.List(true);
            EditorApplication.update += OnPackageListed;
            EditorApplication.QueuePlayerLoopUpdate();
        }

        private static void OnPackageListed()
        {
            EditorApplication.update -= OnPackageListed;
            
            if (_listPackageRequest == null || !_listPackageRequest.IsCompleted || _listPackageRequest.Error != null)
            {
                return;
            }

            foreach (UnityEditor.PackageManager.PackageInfo p in _listPackageRequest.Result)
            {
                if (p.name.Equals("com.unity.addressables"))
                {
                    _isAddressablesInstalled = true;
                    break;
                }
            }

            SetupKeywords();
        }

        private static void SetupKeywords()
        {
            BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
            BuildTargetGroup buildGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);

            string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildGroup);
            List<string> symbolList = new List<string>(symbols.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries));

            var isDirty = SetKeywordActive(symbolList, ADDRESSABLES_KW, _isAddressablesInstalled);

            if (isDirty)
            {
                symbols = ListElementsToString(symbolList, ";");
                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildGroup, symbols);
            }
        }

        private static bool SetKeywordActive(List<string> kwList, string keyword, bool active)
        {
            bool isDirty = false;
            if (active && !kwList.Contains(keyword))
            {
                kwList.Add(keyword);
                isDirty = true;
            }
            else if (!active && kwList.Contains(keyword))
            {
                kwList.RemoveAll(s => s.Equals(keyword));
                isDirty = true;
            }
            return isDirty;
        }

        private static string ListElementsToString<T>(IEnumerable<T> list, string separator)
        {
            IEnumerator<T> i = list.GetEnumerator();
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            while (i.MoveNext())
            {
                s.Append(i.Current).Append(separator);
            }
            return s.ToString();
        }
    }
}
#endif