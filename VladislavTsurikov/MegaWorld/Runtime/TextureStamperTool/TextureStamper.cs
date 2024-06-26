﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Stamper;
using VladislavTsurikov.MegaWorld.Runtime.Common.Utility.Spawn;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.MonoBehaviour;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainTexture;
using VladislavTsurikov.MegaWorld.Runtime.Core.Utility;
#if UNITY_EDITOR
using VladislavTsurikov.MegaWorld.Editor.Common.Stamper;
#endif

namespace VladislavTsurikov.MegaWorld.Runtime.TextureStamperTool
{
    [ExecuteInEditMode]
    [ComponentStack.Runtime.AdvancedComponentStack.Name("Texture Stamper")]
    [SupportMultipleSelectedGroups]
    [SupportedPrototypeTypes(new []{typeof(PrototypeTerrainTexture)})]
    [AddMonoBehaviourComponents(new[]{typeof(TextureStamperArea), typeof(StamperControllerSettings)})]
    [AddGlobalCommonComponents(new []{typeof(LayerSettings)})]
    [AddGeneralPrototypeComponents(typeof(PrototypeTerrainTexture), new []{typeof(MaskFilterComponentSettings)})]
    public class TextureStamper : StamperTool
    {
        private TextureStamperArea _area;
        private StamperControllerSettings _stamperControllerSettings;
        
        public TextureStamperArea Area
        {
            get
            {
                if (_area == null || _area.IsHappenedReset)
                {
                    _area = (TextureStamperArea)GetElement(typeof(TextureStamperArea));
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
        
#if UNITY_EDITOR
        [NonSerialized]
        public StamperVisualisation StamperVisualisation = new StamperVisualisation();
#endif

        [OnDeserializing]
        private void Initialize()
        {
#if UNITY_EDITOR
            StamperVisualisation = new StamperVisualisation();
#endif
        }

        private protected override void OnStamperEnable()
        {
            Area.SetAreaBoundsIfNecessary(this, true);
        }

        protected override void OnUpdate()
        {
            Area.SetAreaBoundsIfNecessary(this);
        }

        protected override async UniTask Spawn(CancellationToken token, bool displayProgressBar)
        {
            int maxTypes = Data.GroupList.Count;
            int completedTypes = 0;
            
            for (int typeIndex = 0; typeIndex < Data.GroupList.Count; typeIndex++)
            {
                token.ThrowIfCancellationRequested();
#if UNITY_EDITOR
                UpdateDisplayProgressBar("Running", "Running " + Data.GroupList[typeIndex].name);
#endif

                RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayDown(transform.position), GlobalCommonComponentSingleton<LayerSettings>.Instance.GetCurrentPaintLayers(Data.GroupList[typeIndex].PrototypeType));
        
                if(rayHit == null)
                {
                    continue;
                }

                BoxArea boxArea = Area.GetAreaVariables(rayHit);
                
                Spawn(Data.GroupList[typeIndex], boxArea);

                completedTypes++;
                SpawnProgress = completedTypes / (float)maxTypes;
                await UniTask.Yield();
            }
        }

        public void SpawnWithCells(List<Bounds> cellList)
        {
            SpawnCancellationTokenSource.Cancel();
            SpawnCancellationTokenSource = new CancellationTokenSource();
            RunSpawnCoroutineWithSpawnCells(SpawnCancellationTokenSource.Token, cellList).Forget();
        }

        private async UniTask RunSpawnCoroutineWithSpawnCells(CancellationToken token, List<Bounds> spawnCellList)
        {
            int maxTypes = Data.GroupList.Count;
            int completedTypes = 0;

            float oneStep = 1 / (float)spawnCellList.Count;

            for (int cellIndex = 0; cellIndex < spawnCellList.Count; cellIndex++)
            {
                float cellProgress = cellIndex / (float)spawnCellList.Count * 100;

                for (int typeIndex = 0; typeIndex < Data.GroupList.Count; typeIndex++)
                {
                    token.ThrowIfCancellationRequested();

                    Bounds bounds = spawnCellList[cellIndex];
                    
                    SpawnProgress = cellProgress / 100;

                    if(maxTypes != 1)
                    {
                        SpawnProgress = cellProgress / 100 + Mathf.Lerp(0, oneStep, completedTypes / (float)maxTypes);
                    }

#if UNITY_EDITOR
                    UpdateDisplayProgressBar(
                        "Cell: " + cellProgress + "%" + " (" + cellIndex + "/" + spawnCellList.Count + ")",
                        "Running " + Data.GroupList[typeIndex].name);
#endif

                    Group group = Data.GroupList[typeIndex];

                    RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayDown(bounds.center), GlobalCommonComponentSingleton<LayerSettings>.Instance.GetCurrentPaintLayers(group.PrototypeType));

                    if(rayHit == null)
                    {
                        continue;
                    }

                    BoxArea boxArea = Area.GetAreaVariablesFromSpawnCell(rayHit, bounds);
                
                    Spawn(group, boxArea);

                    completedTypes++;
                    await UniTask.Yield();
                }
            }
        }
        
        public void Spawn(Group group, BoxArea boxArea)
        {
            if (group.PrototypeType == typeof(PrototypeTerrainTexture))
            {
                SpawnGroup.SpawnTerrainTexture(group, group.PrototypeList, boxArea, 1);
            }
        }
    }
}