using System;
using System.Runtime.Serialization;
using VladislavTsurikov.MegaWorld.Runtime.Common.Stamper.AutoRespawn;
using VladislavTsurikov.MegaWorld.Runtime.Core.MonoBehaviour;
#if UNITY_EDITOR
using VladislavTsurikov.MegaWorld.Editor.Common.Stamper;
#endif

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Stamper
{
    public abstract class StamperTool : MonoBehaviourTool
    {
        public float SpawnProgress;
        public bool CancelSpawn;
        public bool SpawnComplete = true;

#if UNITY_EDITOR
        public AutoRespawnController AutoRespawnController = new AutoRespawnController();
        [NonSerialized]
        public StamperVisualisation StamperVisualisation = new StamperVisualisation();
#endif

        [OnDeserializing]
        private void Initialize()
        {
#if UNITY_EDITOR
            AutoRespawnController = new AutoRespawnController();
            StamperVisualisation = new StamperVisualisation();
#endif
        }

        public abstract void Spawn(bool displayProgressBar = false);
    }
}