using System;
using System.Collections;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime.Scene;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.PhysXPainter.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.ScatterSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Stamper;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Core.MonoBehaviour.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.ElementsSystem.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.Utility;
using Area = VladislavTsurikov.MegaWorld.Runtime.Common.Stamper.Area;
#if UNITY_EDITOR
using VladislavTsurikov.MegaWorld.Editor.GravitySpawner;
#endif

namespace VladislavTsurikov.MegaWorld.Runtime.GravitySpawner
{
    [ExecuteInEditMode]
    [ComponentStack.Runtime.Attributes.MenuItem("Gravity Spawner")]
    [SupportMultipleSelectedGroups]
    [AddMonoBehaviourComponents(new[]{typeof(Area), typeof(StamperControllerSettings), typeof(PhysicsEffects)})]
    [SupportedPrototypeTypes(new []{typeof(PrototypeTerrainObject), typeof(PrototypeGameObject)})]
    [AddGlobalCommonComponents(new []{typeof(LayerSettings)})]
    [AddGeneralPrototypeComponents(new []{typeof(PrototypeGameObject), typeof(PrototypeTerrainObject)}, new []{typeof(SuccessSettings), typeof(PhysicsTransformComponentSettings)})]
    [AddGeneralGroupComponents(new []{typeof(PrototypeGameObject), typeof(PrototypeTerrainObject)}, new []{typeof(RandomSeedSettings), typeof(ScatterComponentSettings)})]
    [AddGroupComponents(new []{typeof(PrototypeGameObject), typeof(PrototypeTerrainObject)}, new []{typeof(FilterSettings)})]
    public class GravitySpawner : StamperTool
    {
        [NonSerialized]
        private Area _area;
        [NonSerialized]
        private StamperControllerSettings _stamperControllerSettings;
        [NonSerialized]
        private PhysicsEffects _physicsEffects;
        
        public Area Area
        {
            get
            {
                if (_area == null || _area.IsHappenedReset)
                {
                    _area = (Area)GetElement(typeof(Area));
                }

                return _area;
            }
        }

        public StamperControllerSettings StamperControllerSettings
        {
            get
            {
                if (_stamperControllerSettings == null || _stamperControllerSettings.IsHappenedReset)
                {
                    _stamperControllerSettings = (StamperControllerSettings)GetElement(typeof(StamperControllerSettings));
                }

                return _stamperControllerSettings;
            }
        }
        
        public PhysicsEffects PhysicsEffects
        {
            get
            {
                if (_physicsEffects == null || _physicsEffects.IsHappenedReset)
                {
                    _physicsEffects = (PhysicsEffects)GetElement(typeof(PhysicsEffects));
                }

                return _physicsEffects;
            }
        }
        
        [NonSerialized] 
        private TerrainsMaskManager _terrainsMaskManager = new TerrainsMaskManager();
        
#if UNITY_EDITOR
        [NonSerialized]
        public GravitySpawnerVisualisation StamperVisualisation = new GravitySpawnerVisualisation();
#endif

        private protected override void OnStamperEnable()
        {
            _terrainsMaskManager = new TerrainsMaskManager();
            
#if UNITY_EDITOR
            StamperVisualisation = new GravitySpawnerVisualisation();
            
            void Action() => StamperVisualisation.StamperMaskFilterVisualisation.NeedUpdateMask = true;
            
            Area.OnSetAreaBounds -= Action;
            Area.OnSetAreaBounds += Action;
#endif

            Area.SetAreaBoundsIfNecessary(this, true);
        }

        protected override void OnUpdate()
        {
            Area.SetAreaBoundsIfNecessary(this);
        }

        protected override IEnumerator Spawn(bool displayProgressBar)
        {
            int maxTypes = Data.GroupList.Count;
            int completedTypes = 0;
            
            for (int i = 0; i < Data.GroupList.Count; i++)
            {
                if (IsCancelSpawn)
                {
                    break;
                }
                
#if UNITY_EDITOR
                UpdateDisplayProgressBar("Running", "Running " + Data.GroupList[i].name,
                    completedTypes / (float)maxTypes);
#endif

                RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayDown(transform.position), GlobalCommonComponentSingleton<LayerSettings>.Instance.GetCurrentPaintLayers(Data.GroupList[i].PrototypeType));
        
                if(rayHit == null)
                {
                    continue;
                }

                BoxArea boxArea = Area.GetAreaVariables(rayHit);

                yield return SpawnGroup(Data.GroupList[i], boxArea);
                
                completedTypes++;
                SpawnProgress = completedTypes / (float)maxTypes;
            }

            SpawnProgress = completedTypes / (float)maxTypes;
        }

        public override void OnCancelSpawn()
        {
            DestroyGameObjectsForPrototypeTerrainObject();
        }
        
        public void DestroyGameObjectsForPrototypeTerrainObject()
        {
            ContainerForGameObjectsUtility.DestroyGameObjects<PrototypeTerrainObject>(Data);
        }

        private IEnumerator SpawnGroup(Group group, BoxArea boxArea)
        {
            if(group.HasAllActivePrototypes())
            {
                if (group.PrototypeType == typeof(PrototypeGameObject))
                {
                    RandomSeedSettings randomSeedSettings = (RandomSeedSettings)group.GetElement(typeof(RandomSeedSettings));
                    randomSeedSettings.GenerateRandomSeedIfNecessary();
                    
                    yield return Utility.SpawnGroup.SpawnGameObject(this, group, _terrainsMaskManager, boxArea);
                }
                else if (group.PrototypeType == typeof(PrototypeTerrainObject))
                {
                    RandomSeedSettings randomSeedSettings = (RandomSeedSettings)group.GetElement(typeof(RandomSeedSettings));
                    randomSeedSettings.GenerateRandomSeedIfNecessary();

                    yield return Utility.SpawnGroup.SpawnTerrainObject(this, group, _terrainsMaskManager, boxArea);
                }
            }
            
            _terrainsMaskManager.Dispose();

            yield return null;
        }
    }
}