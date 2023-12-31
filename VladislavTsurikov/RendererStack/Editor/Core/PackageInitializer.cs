#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace VladislavTsurikov.RendererStack.Editor.Core
{
    public static class PackageInitializer
    {
        private static ListRequest _listPackageRequest;
        private static AddRequest _addPackageRequest;

        public static string BurstKw = "RENDERER_STACK_BURST";

        //Unity packages
        public static bool IsBurstInstalled;
        public static bool IsMathematicsInstalled;

        [DidReloadScripts]
        public static void Init()
        {
            CheckUnityPackagesAndInit();
        }

        private static void CheckUnityPackagesAndInit()
        {
            IsBurstInstalled = false;
            IsMathematicsInstalled = false;

            _listPackageRequest = Client.List(true);
            EditorApplication.update += OnPackageListed;
            EditorApplication.QueuePlayerLoopUpdate();
        }

        private static void OnPackageListed()
        {
            if (_listPackageRequest == null)
                return;
            if (!_listPackageRequest.IsCompleted)
                return;
            if (_listPackageRequest.Error != null)
            {

            }
            else
            {
                foreach (UnityEditor.PackageManager.PackageInfo p in _listPackageRequest.Result)
                {
                    if (p.name.Equals("com.unity.burst"))
                    {
                        IsBurstInstalled = true;
                    }
                    if (p.name.Equals("com.unity.mathematics"))
                    {
                        IsMathematicsInstalled = true;
                    }
                }
            }

            if (!IsMathematicsInstalled)
            {
                _addPackageRequest = Client.Add("com.unity.mathematics");
                EditorApplication.update += OnPackageAdded;
                EditorApplication.QueuePlayerLoopUpdate();
            }
            else
            {
                SetupKeywords();
            }
            EditorApplication.update -= OnPackageListed;
        }

        private static void OnPackageAdded()
        {
            if (_addPackageRequest == null)
                return;
            if (!_addPackageRequest.IsCompleted)
                return;
            if (_addPackageRequest.Error != null)
            {

            }
            else
            {
                Debug.Log($"Dependency package installed [{_addPackageRequest.Result.name}]");
            }

            _listPackageRequest = Client.List(true);
            EditorApplication.update += OnPackageListed;
            EditorApplication.update -= OnPackageAdded;
        }

        private static void SetupKeywords()
        {
            BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
            BuildTargetGroup buildGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);

            string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildGroup);
            List<string> symbolList = new List<string>(symbols.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries));

            bool isDirty = false;

            isDirty = isDirty || SetKeywordActive(symbolList, BurstKw, IsBurstInstalled);

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

        public static string ListElementsToString<T>(IEnumerable<T> list, string separator)
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