#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.UIElementsUtility.Runtime.Core;
using VladislavTsurikov.Utility.Runtime;
using VladislavTsurikov.ReflectionUtility.Runtime;

namespace VladislavTsurikov.UIElementsUtility.Editor.Core
{
    public static class MenuUtility
    {
        [MenuItem("Tools/Vladislav Tsurikov/UIElementsUtility/Generate Default Data Group", false, 0)]
        public static void GenerateDefaultDataGroup()
        {
            foreach (var type in TypeHierarchyHelper.GetDerivedTypes(typeof(DataGroupScriptableObjectCreator<,>)))
            {
                object dataGroupGenerator = Activator.CreateInstance(type);
                
                var methodInfo = type.GetMethod("Run");
                if (methodInfo != null)
                {
                    methodInfo.Invoke(dataGroupGenerator, null);
                }
            }
        }

        [MenuItem("Tools/Vladislav Tsurikov/UIElementsUtility/Generate API", false, 0)]
        public static void GenerateAPI()
        {
            foreach (var type in TypeHierarchyHelper.GetDerivedTypes(typeof(DataGroupAPIGenerator<,>)))
            {
                object dataGroupGenerator = Activator.CreateInstance(type);
                
                var methodInfo = type.GetMethod("Run");
                if (methodInfo != null)
                {
                    methodInfo.Invoke(dataGroupGenerator, null);
                }
            }
        }
    }
}
#endif