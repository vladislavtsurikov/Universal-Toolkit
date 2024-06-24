using System;
using System.Collections;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;
using VladislavTsurikov.Core.Runtime;
using VladislavTsurikov.Coroutines.Runtime;
using VladislavTsurikov.OdinSerializer.Core.Misc;
using VladislavTsurikov.ReflectionUtility.Runtime;
using VladislavTsurikov.Utility.Runtime;
using Coroutine = VladislavTsurikov.Coroutines.Runtime.Coroutine;
using Object = UnityEngine.Object;

namespace VladislavTsurikov.SceneUtility.Runtime
{
    [Serializable]
    public class SceneReference
    {
        public delegate void OnDeleteSceneDelegate();
        public static OnDeleteSceneDelegate OnDeleteScene;
        
        [OdinSerialize] private bool _cachedScene;
        private SceneOperations _sceneOperations;
        
        public Coroutine UnloadSceneCoroutine { get; private set; }
        public Coroutine LoadSceneCoroutine { get; private set; }
        public Coroutine KeepCachedSceneCoroutine { get; private set; }

        public SceneOperations SceneOperations
        {
            get
            {
                if (_sceneOperations != null)
                {
                    return _sceneOperations;
                }

                var types = AllTypesDerivedFrom<SceneOperations>.TypeList
                    .Where(
                        t => !t.IsAbstract
                    );
                
                var typeList = types.ToList();

                foreach (var type in typeList)
                {
                    if (type == typeof(SceneManagerSceneOperations))
                    {
                        continue;
                    }
                    
                    SceneOperations sceneOperations = (SceneOperations)Activator.CreateInstance(type, this);

                    if (!sceneOperations.Enable)
                    {
                        continue;
                    }
                    
                    _sceneOperations = sceneOperations;
                    return _sceneOperations;
                }
                
                _sceneOperations = (SceneManagerSceneOperations)Activator.CreateInstance(typeof(SceneManagerSceneOperations), this);
                return _sceneOperations;
            }
            set => _sceneOperations = value;
        }
        
        public bool CachedScene => _cachedScene;

#if UNITY_EDITOR
        [OdinSerialize]
        private Object _sceneAsset;
#endif
        
        [OdinSerialize]
        private string _scenePath = string.Empty;
        private Scene _scene;
        
#if UNITY_EDITOR
        public Object SceneAsset
        {
            get
            {
                if (_sceneAsset == null)
                {
                    _sceneAsset = string.IsNullOrEmpty(ScenePath) ? null : AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath);
                }

                return _sceneAsset;
            }
            set
            {
                if (value == null)
                {
                    _sceneAsset = null;
                    _scenePath = string.Empty;
                    _scene = new Scene();
                    return;
                }
                
                _sceneAsset = value;
                _scenePath = AssetDatabase.GetAssetPath(_sceneAsset);
                _scene = SceneManager.GetSceneByPath(_scenePath);
            }
        }
#endif


        public string ScenePath
        {
            get
            {
#if UNITY_EDITOR
                if (string.IsNullOrEmpty(_scenePath))
                {
                    _scenePath = _sceneAsset == null ? string.Empty : AssetDatabase.GetAssetPath(_sceneAsset);
                }
#endif
                return _scenePath;
            }
        }


        public Scene Scene
        {
            get
            {
                if (!_scene.IsValid())
                {
                    _scene = SceneManager.GetSceneByPath(_scenePath);
                }

                return _scene;
            }
        }

        public string SceneName => _scenePath.Split('/').Last().Replace(".unity", string.Empty);

        public float LoadingProgress
        {
            get
            {
                if (SceneOperations != null)
                {
                    return SceneOperations.LoadingProgress();
                }

                return Scene.isLoaded ? 1 : 0;
            }
        }
        
        public bool IsLoaded
        {
            get
            {
                
                if (Scene.isLoaded == false)
                {
                    _cachedScene = false;
                    return false;
                }
                
                if (_cachedScene)
                {
                    return false;
                }

                return Scene.isLoaded;
            }
        }

        public SceneReference()
        {
            
        }

        public SceneReference(Scene scene)
        {
            _scene = scene;
            _scenePath = scene.path;
        }
        
#if UNITY_EDITOR
        public SceneReference(SceneAsset sceneAsset)
        {
            SceneAsset = sceneAsset;
        }
#endif

        public IEnumerator LoadScene(float waitForSeconds = 0)
        {
            if (!IsValid())
            {
                yield break;
            }

            if (IsLoaded || SceneOperations.IsLoading())
            {
                yield break;
            }
            
            UnloadSceneCoroutine?.Cancel();
            KeepCachedSceneCoroutine?.Cancel();

            LoadSceneCoroutine = CoroutineRunner.StartCoroutine(Coroutine());
            yield return LoadSceneCoroutine;
            
            IEnumerator Coroutine()
            {
                if (waitForSeconds != 0)
                {
                    yield return new WaitForSeconds(waitForSeconds);
                }

                if (_cachedScene)
                {
                    SetActiveGameObjects(true);
                    _cachedScene = false;
                }
            
#if UNITY_EDITOR
                if (Application.isPlaying)
                {
                    yield return SceneOperations.LoadScene();
                }
                else
                {
                    EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Additive);
                }
#else
                yield return SceneOperations.LoadScene();
#endif
            }
        }
        
        public IEnumerator UnloadScene(float waitForSeconds = 0)
        {
            if (!IsValid())
            {
                yield break;
            }

            if (!IsLoaded || SceneOperations.IsUnloading())
            {
                yield break;
            }
            
            LoadSceneCoroutine?.Cancel();
            KeepCachedSceneCoroutine?.Cancel();
            
            UnloadSceneCoroutine = CoroutineRunner.StartCoroutine(Coroutine());
            yield return UnloadSceneCoroutine;

            IEnumerator Coroutine()
            {
                if (waitForSeconds != 0)
                {
                    yield return new WaitForSeconds(waitForSeconds);
                }

                if (_cachedScene)
                {
                    SetActiveGameObjects(true);
                    _cachedScene = false;
                }
            
#if UNITY_EDITOR
                if (Application.isPlaying)
                {
                    yield return SceneOperations.UnloadScene();
                }
                else
                {
                    if (Scene.isDirty)
                    {
                        EditorSceneManager.SaveScene(Scene);
                    }
                    
                    EditorSceneManager.CloseScene(Scene, true);
                }
#else
                yield return SceneOperations.UnloadScene();
#endif
            }
        }

        public void CacheScene(float waitForSeconds = 0, float keepScene = 0)
        {
            if (IsLoaded)
            {
                if (keepScene == 0 || Profiler.GetTotalReservedMemoryLong() > StreamingUtilitySettings.Instance.GetCacheMemoryThresholdInBytes())
                {
                    CoroutineRunner.StartCoroutine(UnloadScene());
                }
                else
                {
                    UnloadSceneCoroutine = CoroutineRunner.StartCoroutine(Coroutine());
                    
                    IEnumerator Coroutine()
                    {
                        yield return new WaitForSeconds(waitForSeconds);
                        
                        SetActiveGameObjects(false);
                        _cachedScene = true;
                        
                        yield return new WaitForSeconds(keepScene);

                        KeepCachedSceneCoroutine = CoroutineRunner.StartCoroutine(UnloadScene(), this);
                    }
                }
            }
        }

        public void SetActiveGameObjects(bool setActive)
        {
            GameObject[] gameObjects = Scene.GetRootGameObjects();

            foreach (var go in gameObjects)
            {
                go.SetActive(setActive);
            }
        }
        
        public bool IsValid()
        {
#if UNITY_EDITOR
            return SceneAsset != null;
#else
            return true;
#endif
        }

#if UNITY_EDITOR
        public void MarkSceneDirty()
        {
            if(!Application.isPlaying)
            {
                EditorSceneManager.MarkSceneDirty(Scene);
            }
        }

        public void DeleteAsset()
        {
            AssetDatabase.DeleteAsset(ScenePath);
            OnDeleteScene?.Invoke();
        }
#endif
    }
}