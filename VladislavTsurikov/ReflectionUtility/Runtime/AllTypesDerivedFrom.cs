using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace VladislavTsurikov.ReflectionUtility.Runtime
{
    public static class AllTypesDerivedFrom<T>
    {
        public static readonly List<Type> TypeList;

        static AllTypesDerivedFrom()
        {
#if UNITY_EDITOR && UNITY_2019_2_OR_NEWER
            var types = TypeCache.GetTypesDerivedFrom<T>().Where(
                t => !t.IsAbstract
            ); ;
#else
            var types = GetAllAssemblyTypes().Where(t => t.IsSubclassOf(typeof(T)));
#endif

            TypeList = types.ToList();
        }
        
        private static IEnumerable<Type> GetAllAssemblyTypes()
        {
            IEnumerable<Type> assemblyTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(t =>
                {
                    var innerTypes = Type.EmptyTypes;
                    try
                    {
                        innerTypes = t.GetTypes();
                    }
                    catch
                    {
                        // ignored
                    }

                    return innerTypes;
                });

            return assemblyTypes;
        }
    }
}