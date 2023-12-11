using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime.Scene;
using VladislavTsurikov.Coroutines.Runtime;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.OverlapCheckSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.ScatterSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Stamper;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Core.MonoBehaviour.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.ElementsSystem.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainDetail;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainTexture;
using VladislavTsurikov.MegaWorld.Runtime.Core.Utility;
using VladislavTsurikov.MegaWorld.Runtime.TerrainSpawner.Utility;
using Area = VladislavTsurikov.MegaWorld.Runtime.Common.Stamper.Area;

namespace VladislavTsurikov.MegaWorld.Runtime.TerrainSpawner
{
    [ExecuteInEditMode]
    [ComponentStack.Runtime.Attributes.MenuItem("Terrain Spawner")]
    [SupportMultipleSelectedGroups]
    [SupportedPrototypeTypes(new []{typeof(PrototypeTerrainObject), typeof(PrototypeGameObject), typeof(PrototypeTerrainDetail)})]
    [AddMonoBehaviourComponents(new[]{typeof(Area), typeof(StamperControllerSettings)})]
    [AddGlobalCommonComponents(new []{typeof(LayerSettings)})]
    [AddGeneralPrototypeComponents(typeof(PrototypeTerrainObject), new []{typeof(SuccessSettings), typeof(OverlapCheckSettings), typeof(TransformComponentSettings)})]
    [AddGeneralPrototypeComponents(typeof(PrototypeGameObject), new []{typeof(SuccessSettings), typeof(OverlapCheckSettings), typeof(TransformComponentSettings)})]
    [AddGeneralPrototypeComponents(typeof(PrototypeTerrainDetail), new []{typeof(SpawnDetailSettings), typeof(MaskFilterComponentSettings)})]
    [AddGeneralPrototypeComponents(typeof(PrototypeTerrainTexture), new []{typeof(MaskFilterComponentSettings)})]
    [AddGeneralGroupComponents(new []{typeof(PrototypeTerrainObject), typeof(PrototypeGameObject)}, new []{typeof(RandomSeedSettings), typeof(ScatterComponentSettings), typeof(FilterSettings)})]
    public class TerrainSpawner : StamperTool
    {
        [NonSerialized]
        private Area _area;
        [NonSerialized]
        private StamperControllerSettings _stamperControllerSettings;
        
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

        protected override void OnToolEnable()
        {
            Area.SetAreaBoundsIfNecessary(this, true);
        }

        protected override void OnUpdate()
        {
            Area.SetAreaBoundsIfNecessary(this);
        }

        public override void Spawn(bool displayProgressBar = false)
        {
            SpawnComplete = false;
            
            CoroutineRunner.StartCoroutine(RunSpawn(displayProgressBar), this);
        }

        private IEnumerator RunSpawn(bool displayProgressBar)
        {
            CancelSpawn = false;

            int maxTypes = Data.GroupList.Count;
            int completedTypes = 0;
            
            for (int typeIndex = 0; typeIndex < Data.GroupList.Count; typeIndex++)
            {
                if (CancelSpawn)
                {
                    break;
                }
#if UNITY_EDITOR
                if (displayProgressBar)
                {
                    EditorUtility.DisplayProgressBar("Running", "Running " + Data.GroupList[typeIndex].name, completedTypes / (float)maxTypes);
                }
#endif

                RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayDown(transform.position), GlobalCommonComponentSingleton<LayerSettings>.Instance.GetCurrentPaintLayers(Data.GroupList[typeIndex].PrototypeType));
        
                if(rayHit == null)
                {
                    continue;
                }

                BoxArea boxArea = Area.GetAreaVariables(rayHit);

                yield return SpawnGroup(Data.GroupList[typeIndex], boxArea);
                
                completedTypes++;
                SpawnProgress = completedTypes / (float)maxTypes;
            }

            SpawnProgress = completedTypes / (float)maxTypes;
            SpawnProgress = 0;

#if UNITY_EDITOR
            if (displayProgressBar)
            {
                EditorUtility.ClearProgressBar();
            }
#endif
            SpawnComplete = true;
        }
        
        private IEnumerator SpawnGroup(Group group, BoxArea boxArea)
        {
            if(group.HasAllActivePrototypes())
            {
                if (group.PrototypeType == typeof(PrototypeGameObject))
                {
                    RandomSeedSettings randomSeedSettings = (RandomSeedSettings)group.GetElement(typeof(RandomSeedSettings));
                    randomSeedSettings.GenerateNewRandomSeed();
                    
                    yield return Utility.SpawnGroup.SpawnGameObject(group, boxArea);
                }
                else if (group.PrototypeType == typeof(PrototypeTerrainObject))
                {
                    RandomSeedSettings randomSeedSettings = (RandomSeedSettings)group.GetElement(typeof(RandomSeedSettings));
                    randomSeedSettings.GenerateNewRandomSeed();
                    
                    yield return Utility.SpawnGroup.SpawnTerrainObject(group, boxArea);
                }
                else if (group.PrototypeType == typeof(PrototypeTerrainDetail))
                {
                    yield return Utility.SpawnGroup.SpawnTerrainDetails(group, group.PrototypeList, boxArea);
                }
            }
            
            TerrainsMaskManager.Dispose();

            yield return null;
        }
    }
}