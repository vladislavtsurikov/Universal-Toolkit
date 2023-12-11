using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime.Scene;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Stamper;
using VladislavTsurikov.MegaWorld.Runtime.Common.Utility.Spawn;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Core.MonoBehaviour.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.ElementsSystem.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainTexture;
using VladislavTsurikov.MegaWorld.Runtime.Core.Utility;

namespace VladislavTsurikov.MegaWorld.Runtime.TextureStamperTool
{
    [ExecuteInEditMode]
    [ComponentStack.Runtime.Attributes.MenuItem("Texture Stamper")]
    [SupportMultipleSelectedGroups]
    [SupportedPrototypeTypes(new []{typeof(PrototypeTerrainTexture)})]
    [AddMonoBehaviourComponents(new[]{typeof(TextureStamperArea), typeof(StamperControllerSettings)})]
    [AddGlobalCommonComponents(new []{typeof(LayerSettings)})]
    [AddGeneralPrototypeComponents(typeof(PrototypeTerrainTexture), new []{typeof(MaskFilterComponentSettings)})]
    public class TextureStamper : StamperTool
    {
        private IEnumerator _updateCoroutine;
        
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

        protected override void OnToolEnable()
        {
            Area.SetAreaBoundsIfNecessary(this, true);
        }

        protected override void OnUpdate()
        {
            Area.SetAreaBoundsIfNecessary(this);
        }

        public void StartEditorUpdates()
        {
#if UNITY_EDITOR
            EditorApplication.update += EditorUpdate;
#endif
        }

        public void StopEditorUpdates()
        {
  #if UNITY_EDITOR
            EditorApplication.update -= EditorUpdate;
#endif
        }

        private void EditorUpdate()
        {
            if (_updateCoroutine == null)
            {
                StopEditorUpdates();
            }
            else
            {
                _updateCoroutine.MoveNext();
            }
        }
        
        public override void Spawn(bool displayProgressBar = false)
        {
            _updateCoroutine = RunSpawnCoroutine();
            StartEditorUpdates();
        }

        public void SpawnWithCells(List<Bounds> cellList)
        {
            _updateCoroutine = RunSpawnCoroutineWithSpawnCells(cellList);
            StartEditorUpdates();
        }

        public void Spawn(Group group, BoxArea boxArea)
        {            
            if(!IsReadyToSpawn(group))
            {
                return;
            }
            
            if (group.PrototypeType == typeof(PrototypeTerrainTexture))
            {
                SpawnGroup.SpawnTerrainTexture(group, group.PrototypeList, boxArea, 1);
            }
        }

        private bool IsReadyToSpawn(Group group)
        {
            foreach (Prototype proto in group.PrototypeList)
            {
                if(proto.Active)
                {
                    return true;
                }
            }

            return false;
        }

        public void RunSpawn()
        {
            for (int typeIndex = 0; typeIndex < Data.GroupList.Count; typeIndex++)
            {
                Group group = Data.GroupList[typeIndex];

                RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayDown(transform.position), GlobalCommonComponentSingleton<LayerSettings>.Instance.GetCurrentPaintLayers(group.PrototypeType));
        
                if(rayHit == null)
                {
                    return;
                }

                BoxArea boxArea = Area.GetAreaVariables(rayHit);

                Spawn(group, boxArea);
            }
        }

        public IEnumerator RunSpawnCoroutine()
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
                EditorUtility.DisplayProgressBar("Running", "Running " + Data.GroupList[typeIndex].name, completedTypes / (float)maxTypes);
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
                yield return null;
            }

            SpawnProgress = completedTypes / (float)maxTypes;
            yield return null;

            SpawnProgress = 0;

#if UNITY_EDITOR
            EditorUtility.ClearProgressBar();
#endif
            _updateCoroutine = null;
        }

        private IEnumerator RunSpawnCoroutineWithSpawnCells(List<Bounds> spawnCellList)
        {
            CancelSpawn = false;

            int maxTypes = Data.GroupList.Count;
            int completedTypes = 0;

            float oneStep = 1 / (float)spawnCellList.Count;

            for (int cellIndex = 0; cellIndex < spawnCellList.Count; cellIndex++)
            {
                float cellProgress = cellIndex / (float)spawnCellList.Count * 100;

                for (int typeIndex = 0; typeIndex < Data.GroupList.Count; typeIndex++)
                {
                    if (CancelSpawn)
                    {
                        break;
                    }

                    Bounds bounds = spawnCellList[cellIndex];
                    
                    SpawnProgress = cellProgress / 100;

                    if(maxTypes != 1)
                    {
                        SpawnProgress = cellProgress / 100 + Mathf.Lerp(0, oneStep, completedTypes / (float)maxTypes);
                    }

#if UNITY_EDITOR
                    EditorUtility.DisplayProgressBar("Cell: " + cellProgress + "%" + " (" + cellIndex + "/" + spawnCellList.Count + ")", "Running " + Data.GroupList[typeIndex].name, SpawnProgress);
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
                    yield return null;
                }
            }

            SpawnProgress = 1;
            yield return null;

            SpawnProgress = 0;

#if UNITY_EDITOR
            EditorUtility.ClearProgressBar();
#endif
            _updateCoroutine = null;
        }
    }
}