using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.Core.Runtime.Attributes;

namespace VladislavTsurikov.Core.Runtime
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }
                
                T[] singletonInstances = FindObjectsOfType<T>();

                if (singletonInstances.Length == 0)
                {
                    NameAttribute nameAttribute = (NameAttribute)typeof(T).GetAttribute(typeof(NameAttribute));
                    DontDestroyOnLoadAttribute dontDestroyOnLoadAttribute = (DontDestroyOnLoadAttribute)typeof(T).GetAttribute(typeof(DontDestroyOnLoadAttribute));
                        
                    GameObject obj = new GameObject { name = nameAttribute.Name };
                    if (dontDestroyOnLoadAttribute != null)
                    {
                        DontDestroyOnLoad(obj);
                    }
                        
                    _instance = obj.AddComponent<T>();
                }
                else if (singletonInstances.Length > 1)
                {
                    for (int i = 0; i < singletonInstances.Length - 1; i++)
                    {
                        DestroyImmediate(singletonInstances[i]);
                    }
                }
                else
                {
                    _instance = singletonInstances[0];
                }

                return _instance;
            }
        }

        public static T GetInstance()
        {
            return _instance;
        }
        
        public static void DestroyGameObject()
        {
            if (_instance != null)
            {
                DestroyImmediate(_instance.gameObject);
            }
        }
    }
}