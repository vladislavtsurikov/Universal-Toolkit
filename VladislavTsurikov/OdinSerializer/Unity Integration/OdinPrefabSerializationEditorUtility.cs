﻿//-----------------------------------------------------------------------
// <copyright file="OdinPrefabSerializationEditorUtility.cs" company="Sirenix IVS">
// Copyright (c) 2018 Sirenix IVS
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//-----------------------------------------------------------------------

//#define PREFAB_DEBUG

using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
namespace OdinSerializer
{
    public static class OdinPrefabSerializationEditorUtility
    {
        private static bool? hasNewPrefabWorkflow;

        private static readonly MethodInfo PrefabUtility_GetPrefabAssetType_Method =
            typeof(PrefabUtility).GetMethod("GetPrefabAssetType", BindingFlags.Public | BindingFlags.Static, null,
                new[] { typeof(Object) }, null);

        private static readonly MethodInfo PrefabUtility_GetPrefabParent_Method =
            typeof(PrefabUtility).GetMethod("GetPrefabParent", BindingFlags.Public | BindingFlags.Static, null,
                new[] { typeof(Object) }, null);

        private static readonly MethodInfo PrefabUtility_GetCorrespondingObjectFromSource_Method =
            typeof(PrefabUtility).GetMethod("GetCorrespondingObjectFromSource",
                BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(Object) }, null);

        private static readonly MethodInfo PrefabUtility_GetPrefabType_Method =
            typeof(PrefabUtility).GetMethod("GetPrefabType", BindingFlags.Public | BindingFlags.Static, null,
                new[] { typeof(Object) }, null);

        private static readonly MethodInfo PrefabUtility_ApplyPropertyOverride_Method;

        static OdinPrefabSerializationEditorUtility()
        {
            Type interactionModeEnum = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.InteractionMode");

            if (interactionModeEnum != null)
            {
                PrefabUtility_ApplyPropertyOverride_Method = typeof(PrefabUtility).GetMethod("ApplyPropertyOverride",
                    BindingFlags.Public | BindingFlags.Static, null,
                    new[] { typeof(SerializedProperty), typeof(string), interactionModeEnum }, null);
            }
        }

        public static bool HasNewPrefabWorkflow
        {
            get
            {
                if (hasNewPrefabWorkflow == null)
                {
                    hasNewPrefabWorkflow = DetectNewPrefabWorkflow();
                }

                return hasNewPrefabWorkflow.Value;
            }
        }

        public static bool HasApplyPropertyOverride => PrefabUtility_ApplyPropertyOverride_Method != null;

        private static bool DetectNewPrefabWorkflow()
        {
            try
            {
                MethodInfo method = typeof(PrefabUtility).GetMethod("GetPrefabType",
                    BindingFlags.Public | BindingFlags.Static,
                    null, new[] { typeof(Object) }, null);

                if (method == null)
                {
                    return true;
                }

                if (method.IsDefined(typeof(ObsoleteAttribute), false))
                {
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public static void ApplyPropertyOverride(SerializedProperty instanceProperty, string assetPath)
        {
            //PrefabUtility.ApplyPropertyOverride(instanceProperty, assetPath, InteractionMode.AutomatedAction);
            if (!HasApplyPropertyOverride)
            {
                throw new NotSupportedException(
                    "PrefabUtility.ApplyPropertyOverride doesn't exist in this version of Unity");
            }

            PrefabUtility_ApplyPropertyOverride_Method.Invoke(null, new object[] { instanceProperty, assetPath, 0 });
        }

        public static bool ObjectIsPrefabInstance(Object unityObject)
        {
            if (PrefabUtility_GetPrefabType_Method != null)
            {
                try
                {
                    var prefabType =
                        Convert.ToInt32(
                            (Enum)PrefabUtility_GetPrefabType_Method.Invoke(null, new object[] { unityObject }));
                    // PrefabType.PrefabInstance == 3
                    if (prefabType == 3)
                    {
                        return true;
                    }
                }
                catch (Exception)
                {
                }
            }

            if (PrefabUtility_GetPrefabAssetType_Method != null)
            {
                var prefabAssetType =
                    Convert.ToInt32(
                        (Enum)PrefabUtility_GetPrefabAssetType_Method.Invoke(null, new object[] { unityObject }));
                // 1 = PrefabAssetType.Regular
                // 3 = PrefabAssetType.Variant
                return prefabAssetType == 1 || prefabAssetType == 3;
            }

            if (PrefabUtility_GetPrefabType_Method == null && PrefabUtility_GetPrefabAssetType_Method == null)
            {
                Debug.LogError(
                    "Neither PrefabUtility.GetPrefabType or PrefabUtility.GetPrefabAssetType methods could be located. Prefab functionality will likely be broken in this build of Odin.");
            }

            return GetCorrespondingObjectFromSource(unityObject) != null;
        }

        public static bool ObjectHasNestedOdinPrefabData(Object unityObject)
        {
            if (!HasNewPrefabWorkflow)
            {
                return false;
            }

            if (!(unityObject is ISupportsPrefabSerialization))
            {
                return false;
            }

            Object prefab = GetCorrespondingObjectFromSource(unityObject);
            return IsOdinSerializedPrefabInstance(prefab);
        }

        private static bool IsOdinSerializedPrefabInstance(Object unityObject)
        {
            if (!(unityObject is ISupportsPrefabSerialization))
            {
                return false;
            }

            return GetCorrespondingObjectFromSource(unityObject) != null;
        }

        public static Object GetCorrespondingObjectFromSource(Object unityObject)
        {
            if (PrefabUtility_GetCorrespondingObjectFromSource_Method != null)
            {
                return (Object)PrefabUtility_GetCorrespondingObjectFromSource_Method.Invoke(null,
                    new object[] { unityObject });
            }

            if (PrefabUtility_GetPrefabParent_Method != null)
            {
                return (Object)PrefabUtility_GetPrefabParent_Method.Invoke(null, new object[] { unityObject });
            }

            Debug.LogError(
                "Neither PrefabUtility.GetCorrespondingObjectFromSource or PrefabUtility.GetPrefabParent methods could be located. Prefab functionality will be broken in this build of Odin.");
            return null;
        }
    }
}
#endif
