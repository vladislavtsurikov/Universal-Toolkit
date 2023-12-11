using System;
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime.Scene;
using VladislavTsurikov.Math.Runtime;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.RendererData;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.RendererData.ColliderSystem;
using Object = System.Object;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.ScriptingSystem.InstancesSelectorSystem
{
    public abstract class PrototypeInstancesSelector
    {
	    protected readonly ObjectFilter ObjectFilter;
		protected readonly PrototypeTerrainObject PrototypeTerrainObject;

		private readonly List<PrototypeInstancesSelectorData> _prototypeInstancesSelectorDataList = new List<PrototypeInstancesSelectorData>();

		protected PrototypeInstancesSelector(PrototypeTerrainObject proto)
		{
			PrototypeTerrainObject = proto;

			ObjectFilter = new ObjectFilter();
			ObjectFilter.SetFindPrefabs(new List<GameObject> { PrototypeTerrainObject.Prefab });
		}

		public abstract void SetColliders(Sphere sphere, Object usedObj);
		public abstract void OnNewInstanceVisibility(TerrainObjectInstance terrainObjectInstance);
		public abstract void OnInstanceInvisible(TerrainObjectInstance terrainObjectInstance);
		protected abstract void OnRemoveObjectInstanceSelectorData(PrototypeInstancesSelectorData prototypeInstancesSelectorData, object usedObj);
		public abstract Type GetInstanceSelectorData();
		
		protected void EnableCollider(TerrainObjectCollider terrainObjectCollider, Object usedObj)
		{
			if (terrainObjectCollider.Instance.HierarchyTerrainObjectInstance == null)
			{
				OnNewInstanceVisibility(terrainObjectCollider.Instance);
			}

			if (!terrainObjectCollider.Instance.HierarchyTerrainObjectInstance.UsedObjects.Contains(usedObj))
			{
				terrainObjectCollider.Instance.HierarchyTerrainObjectInstance.UsedObjects.Add(usedObj);
			}
		}
		
		protected void DisableCollider(TerrainObjectCollider terrainObjectCollider, Object usedObj)
		{
			if (terrainObjectCollider.Instance.HierarchyTerrainObjectInstance == null)
			{
				return;
			}
			
			terrainObjectCollider.Instance.HierarchyTerrainObjectInstance.UsedObjects.Remove(usedObj);

			if (terrainObjectCollider.Instance.HierarchyTerrainObjectInstance.UsedObjects.Count == 0)
			{
				OnInstanceInvisible(terrainObjectCollider.Instance);
			}
		}

		public PrototypeInstancesSelectorData GetPrototypeInstancesSelectorData(Object usedObj)
		{
			foreach (var item in _prototypeInstancesSelectorDataList)
			{
				if (item.Object == usedObj)
				{
					return item;
				}
			}
			
			PrototypeInstancesSelectorData prototypeInstancesSelectorData = (PrototypeInstancesSelectorData)Activator.CreateInstance(GetInstanceSelectorData(), usedObj);

			_prototypeInstancesSelectorDataList.Add(prototypeInstancesSelectorData);

			return prototypeInstancesSelectorData;
		}

		public void RemovePrototypeInstancesSelectorData(Object usedObj) 
		{
			for (int i = _prototypeInstancesSelectorDataList.Count - 1; i >= 0; i--)
			{
				if (_prototypeInstancesSelectorDataList[i].Object == usedObj)
				{
					OnRemoveObjectInstanceSelectorData(_prototypeInstancesSelectorDataList[i], usedObj);
					_prototypeInstancesSelectorDataList.RemoveAt(i);
				}
			}
		}
	}
}