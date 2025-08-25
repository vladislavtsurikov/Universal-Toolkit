using System.Collections.Generic;
using VladislavTsurikov.MegaWorld.Editor.Core.Window;
using VladislavTsurikov.MegaWorld.Runtime.Core.MonoBehaviour;
using VladislavTsurikov.ReflectionUtility.Runtime;
#if UNITY_EDITOR
#endif

namespace VladislavTsurikov.MegaWorld.Runtime.Core
{
    internal static class AllToolTypes 
    {
        public static readonly List<System.Type> TypeList = new List<System.Type>();

        static AllToolTypes()
        {
#if UNITY_EDITOR
            var toolWindowTypes = AllTypesDerivedFrom<ToolWindow>.Types;
            TypeList.AddRange(toolWindowTypes);
#endif
            var toolMonoBehaviourTypes = AllTypesDerivedFrom<MonoBehaviourTool>.Types;
            TypeList.AddRange(toolMonoBehaviourTypes);
        }
    }
}