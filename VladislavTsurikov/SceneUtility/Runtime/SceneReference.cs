using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using OdinSerializer;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;
using VladislavTsurikov.ReflectionUtility.Runtime;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace VladislavTsurikov.SceneUtility.Runtime
{
    [Serializable]
    public class SceneReference
    {
        [OdinSerialize] private bool _cachedScene;
        private SceneOperations _sceneOperations;
        
#if UNITY_EDITOR
        [OdinSerialize]
        private Object _sceneAsset;
#endif
        
        [OdinSerialize]
        private string _scenePath = string.Empty;
        private Scene _scene;
        
        public static event Action OnDeleteScene;
        
        public CancellationTokenSource UnloadSceneCancellationTokenSource { get; private set; }
        public CancellationTokenSource LoadSceneCancellationTokenSource { get; private set; }
        public CancellationTokenSource KeepCachedSceneCancellationTokenSource { get; private set; }

        public SceneOperations SceneOperations
        {
            get
            {
                if (_sceneOperations != null)
                {
                    return _sceneOperations;
                }

                var types = AllTypesDerivedFrom<SceneOperations>.Types
                    .Where(
                        t => !t.IsAbstract
                    );
                
                foreach (var type in types)
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

        public async UniTask LoadScene(float waitForSeconds = 0)
        {
            if (!IsValid())
            {
                return;
            }

            if (IsLoaded || SceneOperations.IsLoading())
            {
                return;
            }
            
            UnloadSceneCancellationTokenSource?.Cancel();
            KeepCachedSceneCancellationTokenSource?.Cancel();
            
            LoadSceneCancellationTokenSource = new CancellationTokenSource();
            await Load(LoadSceneCancellationTokenSource.Token);

            async UniTask Load(CancellationToken token)
            {
                if (waitForSeconds != 0)
                {
                    await UniTask.WaitForSeconds(waitForSeconds, cancellationToken: token);
                }

                if (_cachedScene)
                {
                    SetGameObjectsAsActive(true);
                    _cachedScene = false;
                }
            
#if UNITY_EDITOR
                if (Application.isPlaying)
                {
                    await SceneOperations.LoadSceneInternal();
                }
                else
                {
                    EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Additive);
                }
#else
                await SceneOperations.LoadSceneInternal();
#endif
            }
        }
        
        public async UniTask UnloadScene(float waitForSeconds = 0)
        {
            if (!IsValid())
            {
                return;
            }

            if (!IsLoaded || SceneOperations.IsUnloading())
            {
                return;
            }
            
            LoadSceneCancellationTokenSource?.Cancel();
            KeepCachedSceneCancellationTokenSource?.Cancel();
            
            UnloadSceneCancellationTokenSource = new CancellationTokenSource();
            await Unload(UnloadSceneCancellationTokenSource.Token);

            async UniTask Unload(CancellationToken token)
            {
                if (waitForSeconds != 0)
                {
                    await UniTask.WaitForSeconds(waitForSeconds, cancellationToken: token);
                }

                if (_cachedScene)
                {
                    SetGameObjectsAsActive(true);
                    _cachedScene = false;
                }
            
#if UNITY_EDITOR
                if (Application.isPlaying)
                {
                    await SceneOperations.UnloadSceneInternal();
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
                await SceneOperations.UnloadSceneInternal();
#endif
            }
        }

        public void CacheScene(float waitForSeconds = 0, float keepScene = 0)
        {
            if (!IsLoaded)
            {
                return;
            }
            
            if (keepScene == 0 || Profiler.GetTotalReservedMemoryLong() > StreamingUtilitySettings.Instance.GetCacheMemoryThresholdInBytes())
            {
                UnloadScene().Forget();
            }
            else
            {
                KeepCachedSceneCancellationTokenSource = new CancellationTokenSource();
                Unload(KeepCachedSceneCancellationTokenSource.Token).Forget();
                    
                async UniTask Unload(CancellationToken token)
                {
                    await UniTask.WaitForSeconds(waitForSeconds, cancellationToken: token);
                        
                    SetGameObjectsAsActive(false);
                    _cachedScene = true;
                        
                    await UniTask.WaitForSeconds(keepScene, cancellationToken: token);
                        
                    await UnloadScene();
                }
            }
        }

        public void SetGameObjectsAsActive(bool setActive)
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