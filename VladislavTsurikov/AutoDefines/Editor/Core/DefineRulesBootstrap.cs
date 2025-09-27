#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEditor.PackageManager;
using UnityEngine;
using VladislavTsurikov.ReflectionUtility.Runtime;

namespace VladislavTsurikov.AutoDefines.Editor.Core
{
    public static class DefineRulesBootstrap
    {
        private static bool s_running;

        [DidReloadScripts]
        private static void OnReloadScripts()
        {
            RegisterEventsOnce();
            UpmListCache.Refresh();
        }

        private static void RegisterEventsOnce()
        {
            Events.registeredPackages += _ => { UpmListCache.Refresh(); };

            UpmListCache.Refreshed += OnUpmListRefreshed;
        }

        private static void OnUpmListRefreshed(UpmListCache.State state)
        {
            if (state != UpmListCache.State.Ready)
            {
                return;
            }

            RunAllRulesOnce();
        }

        private static void RunAllRulesOnce()
        {
            if (s_running)
            {
                return;
            }

            s_running = true;

            IEnumerable<DefineRule> rules = ReflectionFactory.CreateAllInstances<DefineRule>();
            foreach (DefineRule rule in rules)
            {
                try
                {
                    rule.Run();
                }
                catch (SystemException e)
                {
                    Debug.LogException(e);
                }
            }

            s_running = false;
        }
    }
}
#endif
