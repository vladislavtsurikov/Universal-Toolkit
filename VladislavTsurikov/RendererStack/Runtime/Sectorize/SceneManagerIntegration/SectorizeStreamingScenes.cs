using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.SceneDataSystem.Runtime.StreamingUtility;
using VladislavTsurikov.SceneManagerTool.Runtime.SceneCollectionSystem;
using VladislavTsurikov.SceneManagerTool.Runtime.SceneTypeSystem;
using VladislavTsurikov.SceneUtility.Runtime;

namespace VladislavTsurikov.RendererStack.Runtime.Sectorize.SceneManagerIntegration
{
    [Name("Sectorize")]
    public class SectorizeStreamingScenes : SceneType
    {
        private List<Sector> _loadSectors;
        
        public List<SceneReference> SubScenes = new List<SceneReference>();
        
        public static bool StartupLoadComplete { get; private set; }

        protected override async UniTask Load()
        {
            _loadSectors = Sectorize.Instance.GetLoadSectorsForSceneManager();
            
            foreach (var sector in _loadSectors)
            {
                await sector.SceneReference.LoadScene();
            }
            
            StartupLoadComplete = true;
        }

        protected override async UniTask Unload(SceneCollection nextLoadSceneCollection)   
        {
            StartupLoadComplete = false;
            _loadSectors = null;
            await StreamingUtility.UnloadAllScenes(Sectorize.GetSectorLayerTag());
        }
        
        public override bool HasScene(SceneReference sceneReference)
        {
            foreach (var scene in SubScenes)
            {
                if (scene.SceneName == sceneReference.SceneName)
                {
                    return true;
                }
            }

            return false;
        }

        protected override List<SceneReference> GetSceneReferences()
        {
            return SubScenes;
        }

        public override float LoadingProgress()
        {
            if (_loadSectors == null)
            {
                return 0;
            }
            
            return !_loadSectors.Any() ? 1 : _loadSectors.Sum(a => a.SceneReference.LoadingProgress) / _loadSectors.Count;
        }

        public override bool IsValid()
        {
            for (int i = SubScenes.Count - 1; i >= 0; i--)
            {
                if(SubScenes[i] == null || !SubScenes[i].IsValid())
                {
                    SubScenes.RemoveAt(i);
                }
            }
            
            return SubScenes.Any();
        }

        public void AddSubScene(SceneReference sceneReference)
        {
            if (!HasScene(sceneReference))
            {
                SubScenes.Add(sceneReference);
            }
        }
    }
}