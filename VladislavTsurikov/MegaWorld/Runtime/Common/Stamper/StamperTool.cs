using System;
using System.Collections;
using System.Runtime.Serialization;
using VladislavTsurikov.Coroutines.Runtime;
using VladislavTsurikov.MegaWorld.Runtime.Common.Stamper.AutoRespawn;
using VladislavTsurikov.MegaWorld.Runtime.Core.MonoBehaviour;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace VladislavTsurikov.MegaWorld.Runtime.Common.Stamper
{
    public abstract class StamperTool : MonoBehaviourTool
    {
        [NonSerialized]
        private bool _isCancelSpawn;
        [NonSerialized]
        private bool _displayProgressBar;
        [NonSerialized]
        private float _pastProgress;
        
        public float SpawnProgress;

        public bool SpawnComplete
        {
            get
            {
                if (_isCancelSpawn)
                {
                    return true;
                }
                else
                {
                    return SpawnProgress == 1;
                }
            }
        }

        public bool DisplayProgressBar => _displayProgressBar;

        public bool IsCancelSpawn => _isCancelSpawn;

#if UNITY_EDITOR
        public AutoRespawnController AutoRespawnController = new AutoRespawnController();
#endif

        [OnDeserializing]
        private void Initialize()
        {
#if UNITY_EDITOR
            AutoRespawnController = new AutoRespawnController();
#endif
        }
        
        private protected override void OnToolEnable()
        {
            _isCancelSpawn = true;
            OnStamperEnable();
        }

        public void CancelSpawn()
        {
            _isCancelSpawn = true;
            SpawnProgress = 0f;
            
            CoroutineRunner.StopCoroutines(this);
            
#if UNITY_EDITOR
            EditorUtility.ClearProgressBar();
#endif 
            OnCancelSpawn();
        }

        public void StamperSpawn(bool displayProgressBar = false)
        {
            CoroutineRunner.StopCoroutines(this);

            CoroutineRunner.StartCoroutine(StamperSpawnCoroutine(displayProgressBar), this);
        }

        private IEnumerator StamperSpawnCoroutine(bool displayProgressBar = false)
        {
            _isCancelSpawn = false;
            SpawnProgress = 0;
            _displayProgressBar = displayProgressBar;
            
            yield return Spawn();
            
            SpawnProgress = 1;
            _isCancelSpawn = true;

#if UNITY_EDITOR
            EditorUtility.ClearProgressBar();
#endif
        }

#if UNITY_EDITOR
        public void UpdateDisplayProgressBar(string title, string info, float progress = -1)
        {
            if (_displayProgressBar)
            {
                float localProgress = _pastProgress;

                if (progress != -1)
                {
                    localProgress = progress;
                }
                
                EditorUtility.DisplayProgressBar(title, info, localProgress);
                _pastProgress = localProgress;
            }
        }
#endif
        
        public virtual void OnCancelSpawn(){}
        private protected virtual void OnStamperEnable(){}
        
        protected abstract IEnumerator Spawn();
    }
}