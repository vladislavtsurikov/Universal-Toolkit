using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using VladislavTsurikov.Math.Runtime;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.RendererData;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.RendererData.ColliderSystem;
using VladislavTsurikov.SceneDataSystem.Runtime.Utility;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.ScriptingSystem.InstancesSelectorSystem.InstancesSelectors.CellsInstancesSelector
{
    public class CellsPrototypeInstancesSelector : PrototypeInstancesSelector
	{
		private readonly PrototypeTerrainObject _proto;
		
		private readonly ScriptingSystemPool _scriptingSystemPool;

		public CellsPrototypeInstancesSelector(PrototypeScriptingManager prototypeScriptingManager, PrototypeTerrainObject proto) : base(proto)
		{
			_scriptingSystemPool = new ScriptingSystemPool(prototypeScriptingManager);
			_proto = proto;
		}

		public override void OnNewInstanceVisibility(TerrainObjectInstance instance)
		{
			_scriptingSystemPool.Get(instance);
		}

		public override void OnInstanceInvisible(TerrainObjectInstance instance)
		{
			if (instance.HierarchyTerrainObjectInstance == null)
			{
				return;
			}
			
			_scriptingSystemPool.Release(instance.HierarchyTerrainObjectInstance.gameObject);
			instance.HierarchyTerrainObjectInstance = null;
		}
		
		protected override void OnDisableCollider(PrototypeInstancesSelectorData prototypeInstancesSelectorData, object usedObj)
		{
			CellPrototypeInstancesSelectorData cellPrototypeInstancesSelectorData = (CellPrototypeInstancesSelectorData)prototypeInstancesSelectorData;
			
			foreach (var colliderCell in cellPrototypeInstancesSelectorData.LastColliderCellList)
			{
				foreach (var largeObjectCollider in colliderCell.PrototypeBVHObjectTreeStack.GetAllInstances(_proto.ID))
				{
					DisableCollider(largeObjectCollider, usedObj);
				}
			}
		}

		protected override void OnDisableCollider(PrototypeInstancesSelectorData prototypeInstancesSelectorData)
		{
			CellPrototypeInstancesSelectorData cellPrototypeInstancesSelectorData = (CellPrototypeInstancesSelectorData)prototypeInstancesSelectorData;
			
			foreach (var colliderCell in cellPrototypeInstancesSelectorData.LastColliderCellList)
			{
				foreach (var largeObjectCollider in colliderCell.PrototypeBVHObjectTreeStack.GetAllInstances(_proto.ID))
				{
					DisableCollider(largeObjectCollider);
				}
			}
		}

		public override Type GetInstanceSelectorData()
		{
			return typeof(CellPrototypeInstancesSelectorData);
		}

		public override void SetColliders(Sphere sphere, object usedObj)
		{
			CellPrototypeInstancesSelectorData cellPrototypeInstancesSelectorData = (CellPrototypeInstancesSelectorData)GetPrototypeInstancesSelectorData(usedObj);

			Profiler.BeginSample("ColliderUtility.OverlapSphere");

			var newColliderCells = OverlapColliderCell(sphere.Center, sphere.Radius);
				
			Profiler.EndSample();
				
			Profiler.BeginSample("After finding objects");
			
			foreach (var colliderCell in newColliderCells)
			{
				if (cellPrototypeInstancesSelectorData.LastColliderCellList.Contains(colliderCell))
				{
					cellPrototypeInstancesSelectorData.LastColliderCellList.Remove(colliderCell);
					continue;
				}
				
				cellPrototypeInstancesSelectorData.LastColliderCellList.Remove(colliderCell);

				foreach (var largeObjectCollider in colliderCell.PrototypeBVHObjectTreeStack.GetAllInstances(_proto.ID))
				{
					if (largeObjectCollider.Instance.HierarchyTerrainObjectInstance == null)
					{
						EnableCollider(largeObjectCollider, usedObj);
					}
				}
			}

			foreach (var colliderCell in cellPrototypeInstancesSelectorData.LastColliderCellList)
			{
				foreach (var largeObjectCollider in colliderCell.PrototypeBVHObjectTreeStack.GetAllInstances(_proto.ID))
				{
					DisableCollider(largeObjectCollider, usedObj);
				}
			}

			cellPrototypeInstancesSelectorData.LastColliderCellList.Clear();
			cellPrototypeInstancesSelectorData.LastColliderCellList = newColliderCells;
			
			Profiler.EndSample();
		}

		private List<ColliderCell> OverlapColliderCell(Vector3 sphereCenter, float sphereRadius)
		{
			var sectorList = FindSceneDataManager.OverlapSphere(sphereCenter, sphereRadius, Sectorize.Sectorize.GetSectorLayerTag());

			List<ColliderCell> colliderCells = new List<ColliderCell>();
			
			foreach(var node in sectorList)
			{
				TerrainObjectRendererData terrainObjectRendererData = (TerrainObjectRendererData)node.SceneDataStack.GetElement(typeof(TerrainObjectRendererData));

				if (terrainObjectRendererData != null)
				{
					foreach (var cell in terrainObjectRendererData.OverlapCellSphere(sphereCenter, sphereRadius, false))
					{
						colliderCells.AddRange(cell.TerrainObjectRendererCollider.OverlapCellSphere(sphereCenter, sphereRadius, false));
					}
				}
			}
			return colliderCells;
		}
	}
}