using System.Collections.Generic;
using VladislavTsurikov.MegaWorld.Runtime.Core.MonoBehaviour;
using VladislavTsurikov.ReflectionUtility.Runtime;
using VladislavTsurikov.Utility.Runtime;
#if UNITY_EDITOR
using VladislavTsurikov.MegaWorld.Editor.Core.Window;
#endif

namespace VladislavTsurikov.MegaWorld.Runtime.Core
{
    internal static class AllToolTypes 
    {
        public static readonly List<System.Type> TypeList = new List<System.Type>();

        static AllToolTypes()
        {
#if UNITY_EDITOR
            var toolWindowTypes = AllTypesDerivedFrom<ToolWindow>.TypeList;
            TypeList.AddRange(toolWindowTypes);
#endif
            var toolMonoBehaviourTypes = AllTypesDerivedFrom<MonoBehaviourTool>.TypeList;
            TypeList.AddRange(toolMonoBehaviourTypes);
        }
    }
}