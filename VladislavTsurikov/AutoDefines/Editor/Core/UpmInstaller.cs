#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace VladislavTsurikov.AutoDefines.Editor.Core
{
    public static class UpmInstaller
    {
        private static readonly Queue<string> s_queue = new();
        private static AddRequest s_addRequest;
        private static string s_currentPackage;

        public static bool IsInstalling => s_addRequest != null;

        public static void Add(string packageId)
        {
            if (string.IsNullOrWhiteSpace(packageId))
            {
                return;
            }

            if (s_currentPackage == packageId || s_queue.Contains(packageId))
            {
                return;
            }

            s_queue.Enqueue(packageId);
            if (s_addRequest == null)
            {
                StartNext();
            }
        }

        private static void StartNext()
        {
            if (s_queue.Count == 0)
            {
                return;
            }

            s_currentPackage = s_queue.Dequeue();
            s_addRequest = Client.Add(s_currentPackage);
            EditorApplication.update += Tick;
        }

        private static void Tick()
        {
            if (s_addRequest == null)
            {
                EditorApplication.update -= Tick;
                return;
            }

            if (!s_addRequest.IsCompleted)
            {
                return;
            }

            if (s_addRequest.Error != null)
            {
                Debug.LogError($"UPM Add failed: {s_currentPackage} — {s_addRequest.Error.message}");
            }
            else
            {
                Debug.Log($"UPM Add success: {s_currentPackage}");
                UpmListCache.Refresh();
            }

            s_addRequest = null;
            s_currentPackage = null;
            EditorApplication.update -= Tick;
            StartNext();
        }
    }
}
#endif
