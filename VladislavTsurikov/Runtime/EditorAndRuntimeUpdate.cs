using UnityEditor;
using UnityEngine;
using VladislavTsurikov.Core.Runtime.Attributes;
using VladislavTsurikov.Runtime.Attributes;

namespace VladislavTsurikov.Runtime
{
    public static class EditorAndRuntimeUpdate
    {
        public delegate void UpdateEvent();
        private static UpdateEvent _update;
        
        public static void AddListener(UpdateEvent function)
        {
            _update += function;
            if (Application.isPlaying)
            {
                UniversalToolkitRuntimeUpdate universalToolkitRuntimeUpdate = UniversalToolkitRuntimeUpdate.Instance;
            }
            else
            {
#if UNITY_EDITOR
                EditorApplication.update += EditorUpdate;
#endif
            }
        }
        
        public static void RemoveListener(UpdateEvent function)
        {
            _update -= function;
            
            if (Application.isPlaying)
            {
                if (_update == null)
                    UniversalToolkitRuntimeUpdate.DestroyGameObject();
            }
        }

#if UNITY_EDITOR
        private static void EditorUpdate()
        {
            _update?.Invoke();
        }
#endif
        
        [Name("Universal Toolkit Runtime Update")]
        [DontDestroyOnLoad]
        public class UniversalToolkitRuntimeUpdate : MonoSingleton<UniversalToolkitRuntimeUpdate>
        {
            private void Update()
            {
                _update?.Invoke();
            }
        }
    }
}